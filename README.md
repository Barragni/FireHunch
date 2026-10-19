
# FireHunch

**Narrativa interactiva ramificada para fortalecer el conocimiento sobre medidas de respuesta ante incendios estructurales.**

FireHunch es una narrativa interactiva ramificada desarrollada en Unity, cuyo propósito es fortalecer el conocimiento sobre la prevención y las medidas de respuesta ante incendios estructurales. A través de situaciones narrativas, el usuario toma decisiones que influyen en el desarrollo de la experiencia y recibe información relacionada con las consecuencias de sus elecciones.

El proyecto forma parte de una investigación académica enfocada en personas de 18 a 35 años de la localidad de Suba, Bogotá D. C.

## Objetivos

- Fortalecer el conocimiento sobre medidas de respuesta ante incendios estructurales.
- Presentar situaciones que permitan explorar diferentes decisiones ante emergencias.
- Proporcionar retroalimentación sobre las decisiones tomadas durante la experiencia.
- Registrar datos de interacción para apoyar la evaluación de la narrativa interactiva.

## Características principales

- **Narrativa ramificada:** las decisiones del usuario determinan el recorrido narrativo.
- **Escenarios interactivos:** situaciones relacionadas con incendios estructurales y procedimientos de respuesta.
- **Diálogos y secuencias narrativas:** presentación de información mediante personajes, conversaciones y cómics.
- **Retroalimentación:** información sobre las decisiones y sus posibles consecuencias.
- **Interfaz de usuario:** menús, paneles de diálogo, pantallas de resultados y opciones de configuración.
- **Audio:** gestión de música, efectos de sonido y niveles de volumen.
- **Controles de interacción:** elementos que facilitan la navegación y la interacción.
- **Registro de sesiones:** recopilación de puntuaciones, duración, decisiones, aciertos, errores y reintentos.
- **Almacenamiento en MongoDB:** envío de los resultados a un backend para su persistencia en una base de datos.

## Tecnologías utilizadas

### Aplicación

- Unity
- C#
- TextMeshPro
- Unity UI
- NavMesh y NavMeshAgent
- UnityWebRequest

### Backend y almacenamiento

- Node.js
- Express
- MongoDB

## Arquitectura general

La aplicación se desarrolla en Unity y utiliza componentes de C# para controlar la narrativa, la interfaz, los personajes y el registro de las interacciones.

Los resultados se envían mediante solicitudes HTTP a un backend desarrollado con Node.js y Express, que procesa los datos y permite almacenarlos en MongoDB.

```text
Usuario
   |
   v
Narrativa interactiva ramificada
   |
   v
Situaciones y puntos de decisión
   |
   v
Registro de decisiones e interacciones
   |
   v
Preparación de los resultados
   |
   v
Solicitud HTTP al backend
   |
   v
API desarrollada con Node.js y Express
   |
   v
Base de datos MongoDB
```

## Estructura del proyecto

La estructura puede variar según la organización actual del repositorio. En Unity, los principales recursos se encuentran habitualmente dentro de `Assets/`.

```text
FireHunch/
├── Assets/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Materials/
│   ├── Models/
│   ├── Textures/
│   ├── Audio/
│   ├── Animations/
│   └── UI/
├── Packages/
├── ProjectSettings/
├── README.md
└── .gitignore
```

Esta estructura es orientativa y debe ajustarse a las carpetas reales del repositorio.

## Requisitos

Para ejecutar el proyecto se necesita:

- Unity en una versión compatible con el proyecto.
- Git para clonar el repositorio.
- Conexión a internet para enviar resultados al backend.
- Un backend operativo con conexión a MongoDB para almacenar las sesiones.

La versión específica de Unity se puede consultar en `ProjectSettings/ProjectVersion.txt`.

## Instalación y ejecución

### 1. Clonar el repositorio

Abre una terminal y ejecuta:

```bash
git clone https://github.com/Enfuegado/FireHunch.git
```

### 2. Abrir el proyecto

1. Abre Unity Hub.
2. Selecciona **Add** o **Add project from disk**.
3. Busca la carpeta donde clonaste el repositorio.
4. Abre el proyecto con una versión compatible de Unity.
5. Espera a que Unity importe los recursos y procese los archivos del proyecto.

### 3. Ejecutar la aplicación

1. Abre la escena inicial desde el panel `Project`.
2. Comprueba que las referencias y los recursos necesarios estén disponibles.
3. Pulsa **Play** para ejecutar la experiencia dentro del editor.

Para generar una versión ejecutable, utiliza las opciones de compilación disponibles en **File > Build Profiles** o **File > Build Settings**, según la versión de Unity.

## Registro y almacenamiento de datos

FireHunch incorpora mecanismos para registrar información relacionada con las sesiones y las decisiones realizadas durante la experiencia.

Los resultados pueden incluir los siguientes campos:

| Campo | Descripción |
|---|---|
| `date` | Fecha y hora de la sesión. |
| `age` | Edad declarada por el participante. |
| `score` | Puntuación obtenida. |
| `duration` | Duración de la sesión. |
| `ending` | Desenlace o criterio alcanzado. |
| `correctAnswers` | Número de aciertos. |
| `incorrectAnswers` | Número de errores. |
| `retries` | Número de reintentos registrados. |
| `decisionPath` | Secuencia de decisiones realizadas. |

Los nombres anteriores ilustran una posible representación de los datos. Los nombres y tipos definitivos deben coincidir con la implementación de Unity y el esquema que acepta el backend.

### Backend

El backend recibe los resultados enviados por la aplicación y permite almacenarlos en MongoDB.

URL base del backend:

```text
https://firehunchbackend.onrender.com
```

Endpoints utilizados:

| Método | Endpoint | Descripción |
|---|---|---|
| `GET` | `/api/health` | Comprueba el estado del backend. |
| `POST` | `/api/attempts` | Recibe los datos de una sesión. |

#### Ejemplo de envío de datos

Solicitud HTTP:

```http
POST /api/attempts
Content-Type: application/json
```

Cuerpo de la solicitud:

```json
{
  "date": "2026-10-19T03:02:00",
  "age": 18,
  "score": 6.25,
  "duration": 178,
  "ending": "Criterio Insuficiente",
  "correctAnswers": 4,
  "incorrectAnswers": 2,
  "retries": 1,
  "decisionPath": "CCLCLC"
}
```

En este ejemplo, la duración corresponde a 178 segundos, equivalentes a 2 minutos y 58 segundos. Los valores son ilustrativos y no representan datos reales de un participante.

Para probar el endpoint desde PowerShell, puedes utilizar:

```powershell
$body = @{
    date = "2026-10-19T03:02:00"
    age = 18
    score = 6.25
    duration = 178
    ending = "Criterio Insuficiente"
    correctAnswers = 4
    incorrectAnswers = 2
    retries = 1
    decisionPath = "CCLCLC"
} | ConvertTo-Json

Invoke-RestMethod `
    -Uri "https://firehunchbackend.onrender.com/api/attempts" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

La respuesta dependerá de la implementación del backend. Para verificar el servicio, puedes consultar:

```powershell
Invoke-RestMethod `
    -Uri "https://firehunchbackend.onrender.com/api/health" `
    -Method Get
```

Estos ejemplos presuponen que el backend está disponible y acepta los campos enviados. Para que el registro funcione correctamente, la estructura del JSON debe coincidir con la que procesa la API.

## Consideraciones sobre los datos

La información recopilada se utiliza con fines de evaluación académica. Se recomienda:

- Informar a los participantes sobre el propósito de la recopilación de datos.
- Solicitar el consentimiento correspondiente.
- Evitar almacenar información personal innecesaria.
- Restringir el acceso a MongoDB y a las variables de entorno del backend.
- No publicar contraseñas, cadenas de conexión ni credenciales en el repositorio.
- Verificar que el envío y el almacenamiento de los resultados funcionen correctamente.

## Estado del proyecto

FireHunch es un proyecto académico en desarrollo y evaluación. Su funcionamiento depende de la configuración de Unity, los recursos incluidos en el repositorio y la disponibilidad del backend y de MongoDB.

## Contexto académico

El proyecto se enmarca en una investigación que busca fortalecer el conocimiento sobre medidas de respuesta ante incendios estructurales en personas de 18 a 35 años de la localidad de Suba, Bogotá D. C., mediante una narrativa interactiva ramificada.

La experiencia se evalúa como recurso educativo y se compara con un recurso informativo tradicional.

## Autoría

Proyecto desarrollado como parte de una investigación académica.

