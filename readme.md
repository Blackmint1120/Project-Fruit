Project-Fruit

---

### [게임 플레이 (Github Pages)] (https://blackmint1120.github.io/Project-Fruit/)

---

승리의 여신: 니케의 2025년 여름 이벤트 미니게임인 [BFG Clean Up!]을 참고하여 제작한 게임입니다.
학습 및 연습 목적으로 Unity를 통해 제작되었습니다.

---

Personal game project inspired by the minigame [BFG Clean Up!] from [GODDESS OF VICTORY: NIKKE].
This repository contains a Unity 2D implementation created for learning and practice purposes.

---

### 주요 사용 기술

#### 게임 구조 관련
- `GameManager`가 `Playing`, `Paused`, `GameOver`의 세 상태를 관리하며 게임 흐름을 구분
- `GameManager`, `ScoreManager`, `ResolutionManager`를 Singleton, `DontDestroyOnLoad`를 이용하여 구현함으로써 불필요한 전역 변수 최소화
- Paused/GameOver 상태에서만 `RestartGame`, `GoToTitle`을 허용하도록 방어적 조건을 명시하여 안정성 확보

#### 게임플레이 시스템 관련 
- `FruitSpawner`에서 다음 등장 과일을 큐에 저장 및 관리
- 연속 입력 방지를 위해 coroutine을 사용하여 자연스러운 시간 제어 구현
- `LevelManager`의 `LevelData` 구조체를 통해 스폰 범위를 동적으로 조정
- `FruitMerger`가 `OnCollisionEnter2D`에서 자율적으로 병합 판정 및 점수 갱신을 수행
- `GameOverZone`이 독립적으로 게임오버 로직을 처리하여, 물리/게임플로우/UI를 분리

#### 엔진 관련
- `Camera.ScreenToWorldPoint` 기반으로 해상도와 무관한 입력 위치 계산

#### 코드 안정성 관련
- 매니저 간의 상호작용은 반드시 공개된 메서드(`AddScore`, `GameOver`, `TogglePause`)를 통해 수행되며, 직접 참조를 제한
- 주요 변수(`dropLock`, `mergeCooldown`, `spawnYOffset` 등)를 모두 `SerializeField`로 노출하여 게임 밸런싱 용이.
- 비정상적인 상태에 대해 `throw new Exception()`을 명시하여 디버깅 편의성 확보.
- `ScoreManager`만이 TMP UI를 직접 조작하고, 나머지 게임 로직은 완전히 독립.
