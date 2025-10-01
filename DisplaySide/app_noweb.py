import asyncio
import websockets
import json

async def handler(websocket):
    async for message in websocket:
        try:
            data = json.loads(message)
            x = data.get("x")
            y = data.get("y")
            print(f"Received: x={x}, y={y}")

            # 클라이언트(Unity)에 응답 보내기 (텍스트)
            await websocket.send(f"Got your coords: x={x}, y={y}")
        except Exception as e:
            print("Invalid message:", e)

async def main():
    async with websockets.serve(handler, "0.0.0.0", 5000):
        print("WebSocket server started on ws://0.0.0.0:5000")
        await asyncio.Future()  # 서버가 종료되지 않도록 대기

if __name__ == "__main__":
    asyncio.run(main())
