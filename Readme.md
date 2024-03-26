# sl-patch

MeApi.cs 에서 위 패치를 dnspy+ 로 넣는다

radiotape에서 스매시 레전드 티어 오버레이에 사용되는 서버 통신을 넣어줌.

![preview](/preview.png)

obs 등에 오버레이되게끔 사용. 스매시 레전드 버젼이 올라갈때마다 MeApi.cs 변경사항을 보고 패치해야한다

현재 스펙에서는
- 여러개의 계정 지원, 로그인 시 해당 계정으로 변경
- 이름, 티어, 최근 전적, 랭킹 등을 로그인/전투종료 시 업데이트

## CameraController

울트라 와이드에서 카메라가 너무 가까이 있어서 수정함.
CameraController.SetCameraDistance 에서 IL 코드에서 1 + num / 2f에서 1을 1.5로 늘리면 일반 모니터와 비슷한 크기로 된다.