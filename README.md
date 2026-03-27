# VBL-Smart-Crossing
# VBL Smart Crossing

Protótipo desenvolvido como resposta ao Desafio Técnico **Game Developer** do Centro de Pesquisas Avançadas Wernher von Braun.

---

## Pré-requisitos

| Ferramenta | Versão |
|---|---|
| Unity | 6.4 LTS |
| TextMeshPro | Incluído via Package Manager |

---

## Como Rodar

### 1. Clonar o Repositório

```bash
git clone https://github.com/Luxxdev/VBL-Smart-Crossing.git
```

### 2. Abrir no Unity Hub

- Clique em **Add** → selecione a pasta raiz do repositório
- Abra o projeto e aguarde a importação

### 3. Configurar o Mock da API

Os dados de tráfego são lidos de arquivos JSON locais em `Assets/StreamingAssets/`:

```
Assets/StreamingAssets/
├── traffic_easy.json
├── traffic_medium.json
└── traffic_hard.json
```

A cada nível, um arquivo é escolhido aleatoriamente. Para alterar os cenários, edite os arquivos JSON diretamente.

**Estrutura do JSON:**

```json
{
  "current_status": {
    "vehicleDensity": 0.5,
    "averageSpeed": 60.0,
    "weather": "sunny"
  },
  "predicted_status": [
    {
      "estimated_time": 5000,
      "predictions": {
        "vehicleDensity": 0.9,
        "averageSpeed": 30.0,
        "weather": "heavy rain"
      }
    }
  ]
}
```

> **Valores válidos para `weather`:** `sunny` | `clouded` | `foggy` | `light rain` | `heavy rain`  
> **`vehicleDensity`:** float entre `0.1` e `1.0`  
> **`averageSpeed`:** float entre `0` e `100` (km/h)  
> **`estimated_time`:** inteiro em milissegundos

### 4. Dar Play

Abra a cena principal e clique em **Play**. O botão **Play** na HUD inicia o jogo.

---

## Controles

| Tecla | Ação |
|---|---|
| W / ↑ | Avançar (subir faixa) |
| S / ↓ | Recuar |
| A / ← | Mover para a esquerda |
| D / → | Mover para a direita |

---

## Arquitetura

```
Assets/Scripts/
├── TrafficModels.cs         # DTOs — TrafficResponse, TrafficStatus, PredictedEntry
│   TrafficApiService.cs     # Leitura e deserialização do JSON local
│    HUDController.cs         # Nível, cronômetro, dados da API, estados
└── Gameplay/
    ├── GameManager.cs           # Orquestrador: estados, ciclo de vida, predições
    ├── CarSpawner.cs            # Spawn de veículos com fórmulas da API
    ├── Car.cs                   # Movimento e auto-destruição de cada veículo
    ├── Player.cs                # Input, multiplicador de clima, colisões
    └── CameraMovement.cs        # Câmera com threshold e limites

```

---

## Fórmulas Implementadas

**Intervalo de Spawn**
```
Intervalo (s) = 1 / vehicleDensity
```

**Velocidade dos Veículos**
```
Velocidade_Unity = (averageSpeed / 100) × ReferenceSpeed
```

**Multiplicador Climático (jogador)**

| Clima | Multiplicador |
|---|---|
| `sunny` | 1.0× |
| `clouded` / `foggy` | 0.8× |
| `light rain` | 0.6× |
| `heavy rain` | 0.4× |

**Tempo Limite**
```
Tempo = estimated_time da última entrada de predicted_status (ms → s)
```

---

## Ciclo de Jogo

- **Vitória** — jogador alcança a linha de chegada → nível incrementa → novo JSON é carregado
- **Game Over** — cronômetro esgota com o jogador ainda na pista ou jogador colide com veículo

---

## HUD

| Campo | Descrição |
|---|---|
| **Nível** | Número do nível atual |
| **Tempo** | Cronômetro regressivo |
| **Densidade** | `vehicleDensity` recebido da API |
| **Velocidade média** | `averageSpeed` em km/h |
| **Clima** | Condição climática atual |
| **Spawn** | Intervalo calculado: `1 / vehicleDensity` |
| **Vel. carros** | Velocidade Unity calculada |
| **Vel. jogador** | Velocidade base × multiplicador climático |