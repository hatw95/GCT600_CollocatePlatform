from flask import Flask, request, jsonify, render_template, send_from_directory
from flask_sock import Sock
import json
import os

app = Flask(__name__)
sock = Sock(app)

# 전역 변수에 정규화된 좌표를 저장
current_normalized_coordinates = {'x': 0.0, 'y': 0.0}
current_button_index = -1

@app.route('/')
def index():
    return render_template('index3d.html')

@app.route('/get_coordinates')
def get_coordinates():
    # 저장된 정규화된 좌표를 JSON 형식으로 반환
    return jsonify(current_normalized_coordinates)

@sock.route('/ws')
def websocket(ws):
    global current_normalized_coordinates
    global current_button_index
    while True:
        data = ws.receive()
        if data:
            try:
                coords = json.loads(data)
                current_normalized_coordinates['x'] = coords.get('x', 0.0)
                current_normalized_coordinates['y'] = coords.get('y', 0.0)
                print(f"WS Received: x={coords.get('x')}, y={coords.get('y')}")
                ws.send(f"Received coords: x={coords.get('x')}, y={coords.get('y')}")
            except Exception as e:
                ws.send(f"Error: {e}")

            if current_button_index != -1:
                ws.send(f"Received button index: {current_button_index}")

@app.route('/update_button_index', methods=['POST'])
def update_button_index():
    global current_button_index
    data = request.get_json()
    current_button_index = data.get('buttonIndex', -1)
    return jsonify({'success': True})

@app.route('/receive_coordinates', methods=['POST'])
def receive_coordinates():
    global current_normalized_coordinates
    try:
        data = request.get_json()
        print(data)
        if data:
            # Unity로부터 정규화된 좌표를 받아서 저장
            x_coord = data.get('x', 0.0)
            y_coord = data.get('y', 0.0)
            
            current_normalized_coordinates['x'] = x_coord
            current_normalized_coordinates['y'] = y_coord

            print(f"Received normalized coordinates: x={x_coord}, y={y_coord}")
            return jsonify({'status': 'success'})
        else:
            return jsonify({'status': 'error', 'message': 'Invalid JSON data'}), 400
    except Exception as e:
        return jsonify({'status': 'error', 'message': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, host="0.0.0.0", port=5000)