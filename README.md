<h2><b>✨ 시연 영상 바로가기▽ ✨</b></h2>

<a href="https://www.youtube.com/watch?v=MWmmSx7kkx4">
    <img src="https://github.com/user-attachments/assets/62288f3a-c7bc-40d9-b22d-668da9471280" alt="배너를 클릭하면 시연영상 유튜브 링크로 연결됩니다" style="display: block; margin: 0 auto; width: 80%;">
</a>



<h1 align="center">
    <img src="https://github.com/user-attachments/assets/af066f75-b035-4092-9f4a-b7df11760858" alt="Pixelmon Icon" title="Pixelmon" width="5%"/>
    나만의 픽셀몬 키우기
    <img src="https://github.com/user-attachments/assets/af066f75-b035-4092-9f4a-b7df11760858" alt="Pixelmon Icon" title="Pixelmon" width="5%"/>
</h1>

<p align="center">
    <b> Team: 밥도둑 </b>
</p>

---

## 🚀 다운로드 링크
[나만의 픽셀몬 키우기 : Google Play](https://play.google.com/store/apps/details?id=com.teamsparta.unity1Pixelmon&hl=ko "원활한 게임플레이가 가능하고 최신 업데이트가 반영되므로 강력 추천!")&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[나만의 픽셀몬 키우기 : itch.io](https://bobburglar.itch.io/pixelmon-idle "비추천...")

---

![image](https://github.com/user-attachments/assets/1261f457-bcee-4cda-9214-0c71393f495a)

---

## 📖 목차
+ [팀 소개](#팀-소개)
+ [프로젝트 소개](#프로젝트-소개)
+ [깃 컨벤션](#깃-컨벤션)
+ [기능 소개](#기능-소개)
+ [기술적인 도전과제](#기술적인-도전과제)
+ [와이어프레임](#와이어프레임)
+ [UML 다이어그램](#UML-다이어그램)
+ [TroubleShooting](#TroubleShooting)
+ [시연 영상 링크](#시연-영상-링크)
+ [결과 보고서](#결과-보고서)

---

## ✨ 팀 소개

| **이름**   | **직책** |
|------------|----------|
| 성지윤     | ❤️ 팀장 |
| 정승연     | 💙 부팀장 | 
| 이강혁     | 💛 팀원A | 
| 정해성     | 🩷 팀원B |

---

## ✨ 프로젝트 소개

`Project Name` 픽셀몬 키우기

`Info` 사냥을 통해서 알을 얻고, 알을 부화시켜 함께 싸우세요!

`Stack` C#, Unity-2022.3.17f, Visual Studio2022-17.9.6   

- **차별화된 경험 제공**: 귀여운 픽셀몬을 키우는 재미를 핵심 요소로 삼아 방치형 RPG 게임 시장에서 차별화된 경험을 제공합니다.
- **다양한 플레이 스타일 지원**: 조이스틱 이동 기능을 추가하여 조작감을 선호하는 플레이어를 위한 더 다양한 플레이 스타일을 지원합니다.
- **간단한 조작과 접근성**: 간단한 조작과 누구나 쉽게 접근할 수 있는 게임 시스템으로 다양한 연령층을 대상으로 마케팅할 수 있도록 목표했습니다.

[📖 목차로 돌아가기](https://github.com/BobBurglar/PixelmonSourceCode/blob/main/README.md#-%EB%AA%A9%EC%B0%A8)

---

## ✨ 깃 컨벤션

- **Commit 규칙**
    - init: 최초 커밋
    - feat: 기능 추가
    - update: 기능 변경
    - refactor: 구조 개선
    - add: 파일 추가
    - move: 파일 이동, 코드 이동 등
    - remove: 파일 삭제
    - art: UI 개선
    - fix: 버그 수정
    - chore: 기타 잡일
 
- **Branch 전략**
    - dev (하루에 한 번 main 업데이트)
    - 기능마다 1개의 branch.
    - 기능 추가 시: feat/(기능 이름)

[📖 목차로 돌아가기](https://github.com/BobBurglar/PixelmonSourceCode/blob/main/README.md#-%EB%AA%A9%EC%B0%A8)

---

## ✨ 기능 소개

![image](https://github.com/user-attachments/assets/109597db-9576-4cd4-8206-32e071e09365)

[📖 목차로 돌아가기](https://github.com/BobBurglar/PixelmonSourceCode/blob/main/README.md#-%EB%AA%A9%EC%B0%A8)

---

## ✨ 기술적인 도전과제

- **Addressable**: 동적 메모리 관리
- **FSM**: 상태 전환 처리
- **구글 시트 서버 연동**: 빌드 없이 데이터 즉각 반영
- **UI 동적 생성**: 필요한 순간, 즉시 생성
- **Dirty Flag**: 스마트하게 연산 줄이기
- **Async/Await**: 리소스와 데이터 불러오기
- **Observer Pattern**: 반복 호출 No! 한 번만 호출해서 성능 최적화!
- **Extension Method**: 코드 재사용성 Up!
- **UI Particle System**: UI에서 파티클 외않됌?
- **반응성 UI 설계**: 모든 화면에서 최적의 UX (Feat.SafeArea)
- **Object Pool**: 오브젝트 재사용
- **Custom Editor tools**: 한 번만 일하자🐮
- **유사 멀티스레딩, Coroutine**
- **AnimatorHash & Animation Event & Animation Override**
- **맵 끝을 지키는 Cinemachine Camera**
- **Sprite Atlas**: Batch 최적화
- **Generic Singleton**: 상속 싱글톤

[📖 목차로 돌아가기](https://github.com/BobBurglar/PixelmonSourceCode/blob/main/README.md#-%EB%AA%A9%EC%B0%A8)

---

## ✨ 발표 자료

<a href="https://github.com/BobBurglar/PixelmonSourceCode/blob/main/%ED%94%BD%EC%85%80%EB%AA%AC%20%ED%82%A4%EC%9A%B0%EA%B8%B0%20PPT_compressed.pdf">
    <img src="https://github.com/user-attachments/assets/dc592e4d-0e11-45cd-bf40-1903c10c8c4d" alt="Clickable Image">
</a>

[📖 목차로 돌아가기](https://github.com/BobBurglar/PixelmonSourceCode/blob/main/README.md#-%EB%AA%A9%EC%B0%A8)
