# UntilDawn_UPB

> UntilDawn_UPB es una experiencia interactiva en primera persona ambientada en el campus de la UPB. El juego combina exploración, interacción con objetos, narrativa y sistemas de inteligencia artificial para crear una atmósfera de suspenso.

---

## Ambientes

Las siguientes imágenes muestran algunas comparaciones entre las referencias reales del campus UPB y su recreación dentro del videojuego

**Entrada al campus UPB**
![Caseta UPB](Screenshots/Image3.png)

**Edificio DAAE**
![DAAE](Screenshots/Image1.png)

**Entrada a DAAE**
![Entrada a DAAE](Screenshots/Image2.png)

**Bloque A**


![Bloque A](Screenshots/Image4.png)

**Oficina de Alexis**
![Oficina de Alexis](Screenshots/Image5.png)

---

## 1. Descripción general

El jugador es un estudiante que debe explorar el campus intentando encontrar las herramientas necesarias para llegar a su examen y corregirlo. Debe esquivar a los guardias y completar su plan antes de que amanezca y Alexis llegue al campus, para esto debe interactuar con objetos clave, progresar mediante un sistema de misiones, dialogar con NPCs y evadir guardias con patrullaje automático.

El proyecto está centrado en:

- Gameplay en primera persona
- Interacción jugador–entorno
- Sistemas modulares escritos en C#
- UI funcional para guiar al jugador
- Flujo narrativo controlado por scripts

---

## 2. Cómo abrir el proyecto

### Requisitos

- Unity Hub instalado
- Unity Editor en versión compatible con el proyecto

### Pasos

1. Clonar el repositorio:

```bash
git clone https://github.com/seungddaeng/UntilDawn_UPB
```

2. Abrir Unity Hub y seleccionar **Open**, apuntando a la carpeta del proyecto.

3. Abrir la escena principal desde:

```
Assets/Scenes/MainMenu
```

4. Presionar **Play**.

---

## 3. Controles

| Acción | Tecla / Input |
|---|---|
| Mover | `W` `A` `S` `D` |
| Mirar | Mouse |
| Interactuar | `F` |
| Correr | `Shift` |
| Pausar | click derecho |
| Recoger Objetos | `E`  |
| Abrir/Cerrar puertas | `R`  |
| Cambiar examen | `G`  |

---

## 4. Flujo del juego

El flujo está orquestado por los siguientes scripts:

| Script | Función |
|---|---|
| `GameFlowManager.cs` | Control general del estado del juego |
| `StoryTimeManager.cs` | Secuencias narrativas |
| `GameQuestManager.cs` | Progresión de misiones |
| `GameOverController.cs` | Pantalla de derrota |
| `WinScreenController.cs` | Pantalla de victoria |

**Secuencia general:**

```
Menú principal
    → Inicio de escena
    → Introducción narrativa
    → Exploración libre
    → Activación de quests
    → Interacción con NPCs
    → Recolección de objetos
    → Eventos (triggers)
    → Game Over / Victoria
```

---

## 5. Mecánicas principales

### 5.1 Movimiento en primera persona

Scripts: `PlayerMovement.cs`, `PlayerLook.cs`

Proporcionan movimiento fluido, control de cámara y navegación libre por el entorno.

---

### 5.2 Interacción con el entorno

Scripts: `PlayerInteraction.cs`, `PlayerDoorDetector.cs`

Permiten detectar objetos interactivos cercanos y ejecutar acciones contextuales sobre ellos.

---

### 5.3 Sistema de inventario

Script: `PlayerInventory.cs`

Gestiona los objetos recogidos por el jugador, valida el uso de llaves e integra la lógica con otros sistemas del juego.

---

### 5.4 Recolección de objetos

Scripts: `PickupItem.cs`, `QuestPickup.cs`

Objetos recogibles: llaves, baterías y objetos de misión.

---

### 5.5 Sistema de misiones

Scripts: `GameQuestManager.cs`, `QuestTrigger.cs`

Permiten activar objetivos, controlar el progreso del jugador y desbloquear eventos según el avance.

---

### 5.6 Sistema de puertas

Scripts: `Door.cs`, `DoorInteraction_.cs`

Controlan la apertura y cierre de puertas, y verifican si el jugador tiene la llave correspondiente.

---

### 5.7 Sistema de linterna

Script: `FlashlightSystem.cs` — UI: `BatteryUI.cs`

Gestiona el encendido/apagado de la linterna y el consumo de batería, con indicador visual en pantalla.

---

## 6. NPCs e Inteligencia Artificial

### 6.1 Guardias

Script: `GuardPatrol.cs`

Los guardias se mueven entre puntos definidos de forma automática, generando patrullaje continuo.

---

### 6.2 Spawn de NPCs

Script: `RandomSpawnFromPoints.cs`

Permite instanciar NPCs en posiciones predefinidas dentro de la escena.

---

### 6.3 NPCs interactivos

Scripts: `NPCInteractable.cs`, `Alexis.cs`

Habilitan la interacción directa con personajes específicos, activando diálogos o eventos.

---

## 7. Sistema de diálogo

Scripts: `DialogueManager.cs`, `NPCDialogue.cs`, `ConversationTemplate.cs`

**Flujo:**

1. El jugador interactúa con un NPC.
2. Se activa el sistema de diálogo.
3. El texto se muestra progresivamente en la UI.
4. El jugador avanza mediante input.

---

## 8. Interfaz de usuario

| Script | Función |
|---|---|
| `UIMessageManager.cs` | Mensajes informativos al jugador |
| `GuideArrowUI.cs` | Flecha de dirección hacia objetivos |
| `BatteryUI.cs` | Indicador de batería de la linterna |

---

## 9. Sistema de escenas

| Script | Función |
|---|---|
| `MainMenuController.cs` | Menú principal |
| `AutoSceneLoader.cs` | Carga automática de escenas |
| `SkipToNextScene.cs` | Saltar a la siguiente escena |
| `PauseManager.cs` | Pausa del juego |
| `GameOverController.cs` | Pantalla de derrota |
| `WinScreenController.cs` | Pantalla de victoria |

---

## 10. Sistema de audio

Script: `MusicManager.cs`

Controla la reproducción de música ambiental y el ambiente sonoro general de la experiencia.

---

## 11. Sistema narrativo

Script: `StoryTimeManager.cs`

Maneja los eventos narrativos del juego, las secuencias temporales y la activación de momentos clave dentro de la historia.

---

## 12. Tecnologías utilizadas

- **Motor:** Unity Engine
- **Lenguaje:** C#
- **UI:** Sistema de Canvas de Unity
- **Modelado:** ProBuilder (construcción del campus directamente en Unity) y Blender para algunos objetos
- **Física:** Colliders y triggers nativos de Unity
- **Input:** Sistema de input clásico de Unity

---

## 13. Animaciones y eventos

### Animaciones

Script: `CharacterAnimationController.cs`

Controla las animaciones del personaje y las sincroniza con el movimiento del jugador.

### Triggers y eventos

| Script | Función |
|---|---|
| `TriggerEnterUniversity.cs` | Detecta entrada a zonas del campus |
| `QuestTrigger.cs` | Activa misiones y avanza la narrativa |

---

## 14. Estados del juego

Controlados por `GameFlowManager.cs`, `GameOverController.cs` y `WinScreenController.cs`.

| Estado | Descripción |
|---|---|
| Jugando | El jugador tiene control activo |
| Pausado | El juego está detenido temporalmente |
| Game Over | El jugador falló |
| Victoria | El jugador completó los objetivos |

---

## 15. Créditos
Un recorrido por el campus de la UPB, recreado con dedicacion y muchisimo cariño :D

**Desarrollado por:**

- Melany Sonco
- Patricia Quisbert
- Tatiana Aramayo

Entornos Virtuales Multimedios  
Universidad Privada Boliviana

Construido en Unity. Ambientado en la UPB. Sobrevivido en equipo <3
