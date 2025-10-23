# ARP Base Platform

**ARP Base Platform**은 Meta Quest 3 환경에서 현실 공간과 가상 객체를 정합하여 상호작용할 수 있도록 설계된 **Mixed Reality 애플리케이션**입니다.  
사용자는 현실의 스크린 위치를 기준으로 가상의 Plane(스크린)을 조정하고, 로컬 PC와의 네트워크 통신을 통해 실제 버튼 입력 및 3D 오브젝트 출현 등의 동작을 체험할 수 있습니다.

---

## 🏁 실행 개요

### 1️⃣ 공간 스캔
- 앱 실행 전, **Meta Quest 3의 공간 스캔 기능**을 통해 주변 환경을 인식해야 합니다.
- 스캔이 완료되면 앱 실행 시 주변 공간이 **blue effect mesh** 로 시각화됩니다.

### 2️⃣ Plane(가상 스크린) 조작
- Plane은 **현실 스크린의 프록시** 역할을 합니다.
- **오른쪽 컨트롤러 조이스틱**으로 크기를 조정할 수 있습니다.
- **A 버튼**을 눌러 Plane을 실제 공간에 **앵커(anchor)** 합니다.
- 현실 스크린과 Plane이 일치하도록 정렬해야 합니다.
- 정렬 후에는 **효과 메쉬가 자동으로 꺼집니다.**

### 3️⃣ 설정 패널
- 왼손바닥을 바라보면 **Setting 버튼**이 표시됩니다.  
- 버튼을 누르면 **설정 패널**이 사용자 앞에 표시됩니다.
- 설정 패널에서는 다음 항목을 조정할 수 있습니다:
  - IP 주소 입력  
  - 가상 스크린 표시 여부  
  - 네트워킹 기능 사용 여부 (로컬 PC 연결 제어)

---

## 💻 로컬 서버 연동 (DisplaySide 폴더)

### ▶ `app.py` 실행 (웹 UI 버전)
- Python 서버를 실행하면 웹 브라우저에 **3개의 버튼이 있는 페이지**가 열립니다.
- 사용자가 HMD 상의 가상 스크린을 통해 버튼을 터치하면,  
  그 입력이 **로컬 PC로 전송되어 실제 버튼이 클릭**됩니다.
- 어떤 버튼이 눌렸는지 정보가 **PC → HMD로 전송**되어,  
  앱에서 **지정된 3D 오브젝트가 등장**합니다.

> 즉, `app.py`는 **가상 스크린을 통한 상호작용 예제**입니다.

### ▶ `app_noweb.py` 실행 (터미널 버전)
- 웹 인터페이스 없이 터미널에서 **2D 터치 좌표 데이터**를 출력합니다.
- 사용자가 가상 스크린을 터치한 위치를 **픽셀 단위로 실시간 확인**할 수 있습니다.

> `app_noweb.py`는 **터치 포인트 데이터를 수집하거나 디버깅용으로 활용**할 수 있습니다.

---

## ⚙️ 실행 방법

### 1️⃣ HMD 설정
1. Meta Quest 3에서 공간 스캔 완료  
2. 앱 실행 후 Plane을 조정하고 A키로 앵커 고정  
3. 왼손바닥의 Setting 버튼으로 네트워크 및 스크린 표시 설정

### 2️⃣ 로컬 PC 설정
```bash
# Flask 설치 (없다면)
pip install Flask #conda/pip 사용자
# 또는
uv add flask #uv 사용자
# Python 서버 실행
python app.py
# 또는
python app_noweb.py
```

---

## 🧩 팀별 제출 가이드

각 팀은 이 저장소를 **포크(Fork)** 한 후,  
팀명 기준으로 **브랜치(branch)** 와 **팀 폴더**를 생성하여 콘텐츠를 개발합니다.

> ⚙️ 브랜치 명 규칙: `team_<번호>` (https://docs.google.com/spreadsheets/d/1jXcyR9BrMLdj3roGBJjOaDXjW0KfqXn-sSqZlOYZohg/edit?usp=sharing 기준)  
> 예: `team_1`, `team_2`

---

## 📂 프로젝트 구조

모든 팀은 **Unity의 `Assets/teams/<team_name>/` 폴더 내에서만 작업**해야 합니다.  
다른 팀 폴더나 전역 파일(`Assets/Scenes`, `Packages`, `ProjectSettings` 등)은 수정 금지입니다. 만일, 필요하다면 Issue나 메일로 TA에게 연락해주세요. 

---

## 🚀 진행 절차

### 1️⃣ 원본 리포지토리 포크
1. 우측 상단의 **Fork** 버튼 클릭  
2. 본인 계정으로 복제
3.
```bash
git clone https://github.com/<your-github-id>/GCT600_CollocatePlatform.git
cd GCT600_CollocatePlatform

git remote add upstream https://github.com/hatw95/GCT600_CollocatePlatform.git
git checkout -b team_1
```
4. Assets/teams/team_{팀번호}/ 생성
구성 예시
Assets/teams/team_1/
 ┣ Scenes/
 ┣ Scripts/
 ┣ Prefabs/
 ┣ Materials/
 ┗ README.md
README.md 예시
```markdown
# Team 1

## 팀원
- 홍길동 (@hong)
- 김영희 (@kim)

## 개요
본 폴더는 Team 1의 Unity 콘텐츠를 포함합니다.

## 실행 방법
1. Unity 2022.3.x 이상 버전으로 프로젝트 열기  
2. `Assets/teams/team_1/Scenes/Main.unity` 실행

## 변경 내역
- 초기 환경 세팅
- Plane Proxy Prefab 추가
```
5.
```bash
git add .
git commit -m "[team_1] add initial Unity scene"
git push -u origin team_1
```

6. Pull Request 생성
GitHub에서 포크된 저장소로 이동
상단의 Compare&pull request 클릭
제목과 설명을 수정 후 Create pull requeset 버튼을 클릭

---

⚠️ 주의 사항

반드시 자신의 팀 브랜치(team_x) 에서만 작업하세요.
main 브랜치에는 직접 커밋하지 않습니다.
팀 폴더 외부 수정 금지! (충돌 및 merge 거부 사유)
대용량 파일(>100MB)은 업로드 금지 — 필요시 Git LFS 또는 외부 링크 사용
외부 에셋 사용 시 출처와 라이선스를 README.md에 반드시 명시

---

최신 코드 동기화
```bash
git fetch upstream
git checkout team_1
git merge upstream/main
git push
```

