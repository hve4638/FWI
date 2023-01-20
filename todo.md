## TODO

### FWIConnection

[ ] 암호화 통신 지원

### FWI

[?] Console.WriteLine 사용하는 대신 Results<string> 사용

[?] FormatStandardOutput 버그 수정

### 클라이언트

[X] 연결끊길시 재연결 시도

[ ] 백그라운드 실행 & Form 추가

[X] AFK 체크해서 보내기

[X] 연결 종료시 trace, prompt 스레드 안전하게 종료

[ ] 강제 종료하는 대신, 정상 종료하기

[ ] 서버로부터 AFK 처리 시간 받기

### 서버

[X] 콘솔 출력시 현재시각, 클라이언트 정보 출력

[X] 1분마다 Trace 권한 존재하는지 체크

[X] Program.cs 리팩토링

[X] Prompt 기본 OutputStream 지정

[X] AFK 처리

[X] import시 잘못된 경로를 찾는 문제

[ ] MultiLogger

[ ] VerboseOut Program에

[ ] FWIManager 에서 Target관리하는 대신 ServerManager 를 새로 만들기

[ ] Youtube Export 기능 만들기

[ ] Target Client 해제시 AFK상태로

[ ] AFK 상태 체크 (AFK 도중 UpdateWI시 AFK 해제)