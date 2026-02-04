# Arquitectura de la Solución - Winni Electricidad

## Índice
1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Diagrama General de la Solución](#diagrama-general-de-la-solución)
3. [Arquitectura del Backend](#arquitectura-del-backend)
4. [Arquitectura del Frontend](#arquitectura-del-frontend)
5. [Modelo de Datos](#modelo-de-datos)
6. [Diagrama de Componentes](#diagrama-de-componentes)
7. [Diagrama de Despliegue](#diagrama-de-despliegue)
8. [Integraciones Externas](#integraciones-externas)
9. [Flujos Principales](#flujos-principales)
10. [Stack Tecnológico](#stack-tecnológico)

---

## Resumen Ejecutivo

Winni Electricidad es una plataforma web para la gestión integral de servicios eléctricos, desarrollada como una aplicación moderna de tres capas con arquitectura separada Frontend-Backend.

**Características principales:**
- Sistema de reservas y agendamiento de servicios eléctricos
- Gestión de presupuestos y pagos integrados con MercadoPago
- Sistema de reseñas con moderación automática usando IA (OpenAI)
- Panel administrativo completo para gestión de clientes y servicios
- Autenticación JWT con roles diferenciados (Cliente/Administrador)

---

## Diagrama General de la Solución

```
┌─────────────────────────────────────────────────────────────────────┐
│                         USUARIOS FINALES                            │
│                  (Clientes y Administradores)                       │
└────────────────────────┬────────────────────────────────────────────┘
                         │ HTTPS
                         ↓
┌─────────────────────────────────────────────────────────────────────┐
│                    FRONTEND - React + Vite                          │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐       │
│  │  Páginas       │  │  Componentes   │  │   Servicios    │       │
│  │  Públicas      │  │      UI        │  │      API       │       │
│  └────────────────┘  └────────────────┘  └────────────────┘       │
│           Azure Static Web Apps (puerto 3001)                       │
└────────────────────────┬────────────────────────────────────────────┘
                         │ REST API (JSON)
                         │ JWT Bearer Token
                         ↓
┌─────────────────────────────────────────────────────────────────────┐
│                 BACKEND - .NET 8 Web API                            │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                  Capa de Presentación                        │  │
│  │  Controllers: Usuario, Reserva, Servicio, Reseña, etc.      │  │
│  └────────────────────────┬─────────────────────────────────────┘  │
│                           │                                         │
│  ┌────────────────────────▼─────────────────────────────────────┐  │
│  │              Capa de Lógica de Aplicación                    │  │
│  │  42 Servicios de Aplicación (Use Cases)                      │  │
│  └────────────────────────┬─────────────────────────────────────┘  │
│                           │                                         │
│  ┌────────────────────────▼─────────────────────────────────────┐  │
│  │              Capa de Acceso a Datos                          │  │
│  │  Patrón Repository + Entity Framework Core                   │  │
│  └────────────────────────┬─────────────────────────────────────┘  │
│                           │                                         │
│  ┌────────────────────────▼─────────────────────────────────────┐  │
│  │              Capa de Lógica de Negocio                       │  │
│  │  17 Entidades de Dominio + Validaciones                      │  │
│  └──────────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────────┘
                         │
        ┌────────────────┼────────────────┬────────────────┐
        ↓                ↓                ↓                ↓
┌───────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│  SQL Server   │ │  Azure Blob  │ │  MercadoPago │ │   OpenAI     │
│   Database    │ │   Storage    │ │     API      │ │     API      │
└───────────────┘ └──────────────┘ └──────────────┘ └──────────────┘
```

**Descripción:**
- **Frontend React SPA**: Interfaz de usuario responsiva con Material-UI
- **Backend .NET API REST**: Arquitectura en capas con separación de responsabilidades
- **Base de Datos**: SQL Server con Entity Framework Core
- **Servicios Externos**: Azure (imágenes), MercadoPago (pagos), OpenAI (moderación)

---

## Arquitectura del Backend

### Estructura de Capas

El backend sigue una **arquitectura limpia en capas** con las siguientes responsabilidades:

```
┌────────────────────────────────────────────────────────────────┐
│  WinniElectricidad.Api                  (Capa de Presentación)│
│  - Controllers REST                                            │
│  - Configuración de JWT, CORS, Swagger                        │
│  - Inyección de Dependencias                                  │
└────────────────────────┬───────────────────────────────────────┘
                         │
┌────────────────────────▼───────────────────────────────────────┐
│  WinniElectricidad.LogicaAplicacion  (Capa de Aplicación)     │
│  - 42 Servicios de Aplicación (Use Cases)                     │
│  - Orquestación de lógica de negocio                          │
│  - Interfaz: IServicio → Implementación                       │
└────────────────────────┬───────────────────────────────────────┘
                         │
┌────────────────────────▼───────────────────────────────────────┐
│  WinniElectricidad.AccesoDatos     (Capa de Acceso a Datos)   │
│  - Patrón Repository (9 repositorios)                         │
│  - Entity Framework Core DbContext                            │
│  - Migraciones de base de datos                               │
└────────────────────────┬───────────────────────────────────────┘
                         │
┌────────────────────────▼───────────────────────────────────────┐
│  WinniElectricidad.LogicaNegocio  (Capa de Dominio)           │
│  - 17 Entidades de Dominio                                     │
│  - Validaciones de negocio                                     │
│  - Excepciones personalizadas                                  │
│  - Enumeraciones                                               │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  WinniElectricidad.Compartido      (Capa Transversal)         │
│  - DTOs (Data Transfer Objects)                                │
│  - Mappers (Entity → DTO)                                      │
│  - Opciones de Configuración                                   │
└────────────────────────────────────────────────────────────────┘
```

### Componentes Principales

#### 1. **Controllers (API)**
Exponen endpoints REST para cada dominio:

| Controller | Endpoints | Responsabilidad |
|-----------|-----------|-----------------|
| `UsuarioController` | 9 endpoints | Autenticación, registro, gestión de usuarios |
| `ReservaController` | 11 endpoints | Agendamiento, aprobación, cancelación |
| `ServicioController` | 5 endpoints | CRUD de servicios eléctricos |
| `ReseñaController` | 3 endpoints | Creación y moderación de reseñas |
| `PresupuestoController` | 4 endpoints | Gestión de presupuestos |
| `PagoController` | - | Registro de pagos |
| `NotificacionController` | 2 endpoints | Envío de emails y recordatorios |
| `ConsultaController` | 1 endpoint | Consultas públicas |
| `MercadoPagoWebhookController` | 1 endpoint | Procesamiento de webhooks |

#### 2. **Servicios de Aplicación (42 servicios)**

Organizados por dominio con patrón de interfaz/implementación:

**Usuario (14 servicios):**
- Autenticación (Login, JWT)
- Registro con HCaptcha
- Recuperación de contraseña
- Búsqueda y listado de usuarios
- Gestión de direcciones

**Reserva (9 servicios):**
- Agendamiento con validaciones de negocio
- Cálculo de disponibilidad horaria
- Aprobación/cancelación/modificación
- Históricos por estado y período

**Reseña (5 servicios):**
- Creación con carga de imágenes
- Moderación automática con OpenAI
- Aprobación/desaprobación manual

**Otros dominios:**
- Presupuesto (3), Pago (3), Notificación (2), Servicio (5), Consulta (1)

#### 3. **Repositorios (Patrón Repository)**

```
IRepositorio<T>  (Interfaz Genérica)
    ├── Obtener(id)
    ├── ObtenerTodos()
    ├── Agregar(entidad)
    ├── Actualizar(entidad)
    └── Eliminar(id)

Repositorios Específicos:
    ├── IRepositorioUsuario
    ├── IRepositorioReserva
    ├── IRepositorioServicio
    ├── IRepositorioReseña
    ├── IRepositorioPresupuesto
    ├── IRepositorioPago
    ├── IRepositorioOneTimeToken
    ├── IRepositorioSettings
    └── IRepositorioNotificacion
```

#### 4. **Entidades de Dominio (17 entidades)**

**Jerarquía de Usuarios (TPH - Table Per Hierarchy):**
```
UsuarioBase (Abstracta)
    ├── UsuarioCliente
    │   └── Relaciones: Direcciones, Reservas, Pagos, Presupuestos, Reseñas
    └── UsuarioAdministrador
        └── Relaciones: NotificacionesEnviadas
```

**Entidades de Negocio:**
- `Reserva`: Validación de 48h anticipación, máximo 30 días, no domingos
- `Servicio`: Servicios eléctricos activos/inactivos
- `ServicioImagen`: Imágenes en Azure Blob Storage
- `Reseña`: Puntuación 1-5, moderación por IA
- `Presupuesto`: Vinculado a reserva
- `Pago`: Integración con MercadoPago
- `Direccion`: Dirección del cliente
- `Notificacion`: Notificaciones por email
- `OneTimeToken`: Tokens para recuperación de contraseña
- `Settings`: Configuración del sistema

**Enumeraciones:**
- `EstadoReserva`: Pendiente, Confirmada, Cancelada
- `EstadoPago`: Approved, Rejected, Pending, Refunded
- `EstadoReseña`: Aprobada, Desaprobada
- `TipoServicioReserva`: Instalación, Mantenimiento

### Patrones de Diseño Aplicados

1. **Repository Pattern**: Abstracción del acceso a datos
2. **Dependency Injection**: Inversión de control en toda la aplicación
3. **Service Layer**: Encapsulación de lógica de aplicación
4. **DTO Pattern**: Separación entre dominio y contratos de API
5. **Factory Pattern**: Creación de entidades complejas
6. **Strategy Pattern**: Servicios externos intercambiables
7. **Configuration Pattern**: IOptions<T> para configuraciones tipadas
8. **Async/Await**: Operaciones asíncronas en todas las capas


## Arquitectura del Frontend

### Estructura de Directorios

```
src/
├── app/
│   └── slices/           # Estado global (Redux minimal)
├── components/
│   ├── App/              # Componente raíz con enrutamiento
│   ├── Header/           # Barra de navegación superior
│   ├── SideBar/          # Menú lateral con contexto
│   ├── UI/               # Componentes reutilizables (Button, Loader)
│   ├── CustomIcons/      # Íconos personalizados
│   ├── Domain/           # Componentes específicos de dominio
│   └── routes/           # Guardias de ruta (ProtectedRoute)
├── pages/
│   ├── Public/           # Landing, Login, Registro, Servicios
│   ├── User/             # Dashboard, Agenda, Reservas, Pago
│   └── Admin/            # Panel, Fichas, Recordatorios
├── services/             # Integración con API Backend
├── layout/               # Layout principal (MainLayout)
└── shared-theme/         # Tema Material-UI
```

### Rutas y Páginas

#### **Rutas Públicas** (sin autenticación)
```
/                      → Principal (Landing page)
/login                 → Autenticación
/registro              → Registro de clientes con HCaptcha
/servicios             → Catálogo de servicios
/resenas               → Reseñas públicas
/forgot-password       → Recuperar contraseña
/reset-password        → Restablecer contraseña
```

#### **Rutas de Cliente** (rol: "Cliente")
```
/cliente
├── /                  → Dashboard del cliente
├── /agenda            → Agendar nueva reserva
├── /mis-reservas      → Historial de reservas
├── /servicios         → Catálogo de servicios
├── /resenas           → Ver y crear reseñas
├── /resenas/crear     → Formulario de reseña
├── /pago/crear        → Crear pago con MercadoPago
├── /success           → Pago exitoso
├── /pending           → Pago pendiente
└── /failure           → Pago fallido
```

#### **Rutas de Administrador** (rol: "Administrador")
```
/admin
├── /                  → Dashboard administrativo
├── /panel-reservas    → Calendario semanal de reservas
├── /crear-reserva     → Crear reserva histórica
├── /ficha-clientes    → Base de datos de clientes
├── /servicios         → Gestión de servicios
├── /resenas           → Moderación de reseñas
└── /recordatorios     → Envío de recordatorios estacionales
```

### Componentes Clave

#### **Guardias de Ruta**
```javascript
<ProtectedRoute requiredRole="Cliente">
  <ClientePage />
</ProtectedRoute>
```
- Verifica token JWT en localStorage
- Valida rol del usuario
- Redirige a login si no está autenticado

#### **Gestión de Estado**
- **Local State**: `useState()` para datos de componente
- **Context API**: `SidebarContext` para estado del menú
- **localStorage**: Token JWT, rol, ID de usuario
- **No Redux global**: Enfoque minimalista

#### **Servicios de API**
Módulos en `/services/` que encapsulan llamadas REST:

```javascript
// authService.js
login(email, password)
register(userData)
resetPassword(token, newPassword)

// reservaService.js
fetchReservasByMonth(year, month)
approveReserva(idReserva)
cancelReserva(idReserva)

// pagoService.js
createPaymentOrder(presupuestoId, amount)
```

Todos con:
- Bearer token automático desde localStorage
- Manejo de errores con `ApiError`
- AbortController para cancelación

---

## Modelo de Datos

### Diagrama de Entidad-Relación (MER)

```
┌─────────────────────┐
│   UsuarioBase       │ (TPH: UsuarioCliente / UsuarioAdministrador)
│─────────────────────│
│ IdUsuario (PK)      │
│ Email               │◄────────┐
│ Password (Hash)     │         │
│ Nombre              │         │
│ Apellido            │         │
│ Telefono            │         │
│ Discriminator       │         │
└──────┬──────────────┘         │
       │ 1                      │
       │                        │
       │ N                      │ 1
┌──────▼──────────────┐         │
│   Direccion         │         │
│─────────────────────│         │
│ IdDireccion (PK)    │         │
│ Calle               │         │
│ Esquina             │         │
│ Apartamento         │         │
│ Aclaraciones        │         │
│ IdUsuario (FK)      │─────────┘
└─────────────────────┘

┌─────────────────────┐
│   Reserva           │
│─────────────────────│
│ IdReserva (PK)      │
│ FechaHoraInicio     │
│ FechaHoraFin        │
│ Estado              │◄─── EstadoReserva (enum)
│ Descripcion         │
│ TipoServicio        │◄─── TipoServicioReserva (enum)
│ IdUsuario (FK)      │─────┐
│ IdDireccion (FK)    │     │
└──────┬──────────────┘     │
       │ N:M                │ 1
       │                    │
┌──────▼──────────────┐     │
│   Servicio          │     │
│─────────────────────│     │
│ Id (PK)             │     │
│ Titulo              │     │
│ Descripcion         │     │
│ Activo              │     │
└──────┬──────────────┘     │
       │ 1                  │
       │                    │
       │ N                  │
┌──────▼──────────────┐     │
│   ServicioImagen    │     │
│─────────────────────│     │
│ Url (Azure Blob)    │     │
│ IdServicio (FK)     │     │
└─────────────────────┘     │
                            │
┌─────────────────────┐     │
│   Presupuesto       │     │
│─────────────────────│     │
│ Id (PK)             │     │
│ Monto               │     │
│ FechaCreacion       │     │
│ IdReserva (FK)      │     │
│ IdUsuario (FK)      │─────┘
└──────┬──────────────┘
       │ 1
       │
       │ N
┌──────▼──────────────┐
│   Pago              │
│─────────────────────│
│ IdPago (PK)         │
│ Monto               │
│ Estado              │◄─── EstadoPago (enum)
│ MercadoPagoId       │
│ FechaPago           │
│ IdPresupuesto (FK)  │
│ IdUsuario (FK)      │
└─────────────────────┘

┌─────────────────────┐
│   Reseña            │
│─────────────────────│
│ Id (PK)             │
│ Puntuacion (1-5)    │
│ Descripcion         │
│ UrlImagen           │
│ Estado              │◄─── EstadoReseña (enum)
│ FechaCreacion       │
│ IdUsuario (FK)      │
│ IdServicio (FK)     │
└─────────────────────┘

┌─────────────────────┐
│   OneTimeToken      │
│─────────────────────│
│ Id (PK)             │
│ Hash (Indexed)      │
│ FechaExpiracion     │
│ Usado               │
│ IdUsuario (FK)      │
└─────────────────────┘

┌─────────────────────┐
│   Notificacion      │
│─────────────────────│
│ IdNotificacion (PK) │
│ Asunto              │
│ Mensaje             │
│ FechaEnvio          │
│ IdUsuario (FK)      │
│ IdEmisor (FK)       │
└─────────────────────┘
```

### Relaciones Principales

1. **Usuario → Direccion** (1:N): Un usuario puede tener múltiples direcciones
2. **Usuario → Reserva** (1:N): Un cliente puede tener múltiples reservas
3. **Reserva ↔ Servicio** (N:M): Una reserva puede incluir múltiples servicios
4. **Reserva → Presupuesto** (1:1): Cada reserva tiene un presupuesto
5. **Presupuesto → Pago** (1:N): Un presupuesto puede tener múltiples pagos
6. **Usuario → Reseña** (1:N): Un usuario puede crear múltiples reseñas
7. **Servicio → Reseña** (1:N): Un servicio puede tener múltiples reseñas
8. **Usuario → OneTimeToken** (1:N): Tokens para recuperación de contraseña

### Constraints y Validaciones

**A nivel de Base de Datos:**
- PK: Auto-incrementales en todas las entidades
- FK: Restricciones de integridad referencial
- Unique: Email de usuario, Hash de token
- Check: Puntuación reseña (1-5), Monto > 0

**A nivel de Dominio:**
- Reserva: Mínimo 48h de anticipación, máximo 30 días, no domingos
- Servicio: Título único, descripción mínimo 15 caracteres
- Reseña: Descripción no vacía, puntuación 1-5
- Usuario: Email válido, teléfono formato correcto

---

## Diagrama de Componentes

```
┌────────────────────────────────────────────────────────────────────────────┐
│                           COMPONENTES DEL SISTEMA                          │
└────────────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│  FRONTEND (React SPA)                                                      │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────────┐       │
│  │  Módulo Público  │  │  Módulo Cliente  │  │  Módulo Admin     │       │
│  │  - Landing       │  │  - Dashboard     │  │  - Panel Reservas │       │
│  │  - Login         │  │  - Agenda        │  │  - Ficha Clientes │       │
│  │  - Registro      │  │  - Mis Reservas  │  │  - Gestión Svc    │       │
│  │  - Servicios     │  │  - Pago MP       │  │  - Moderación     │       │
│  └──────────────────┘  └──────────────────┘  └───────────────────┘       │
│                                                                            │
│  ┌──────────────────────────────────────────────────────────────────────┐ │
│  │  Servicios de API (authService, reservaService, pagoService, etc.)  │ │
│  └──────────────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────┬───────────────────────────────────────┘
                                     │ HTTPS / JWT
                                     ↓
┌────────────────────────────────────────────────────────────────────────────┐
│  BACKEND API (.NET 8)                                                      │
│  ┌──────────────────────────────────────────────────────────────────────┐ │
│  │  Controllers Layer                                                   │ │
│  │  - UsuarioController    - ReservaController   - ServicioController  │ │
│  │  - ReseñaController     - PagoController      - PresupuestoCtrl     │ │
│  │  - NotificacionCtrl     - ConsultaController  - WebhookController   │ │
│  └──────────────────────────────────────────────────────────────────────┘ │
│                                     │                                      │
│  ┌──────────────────────────────────▼──────────────────────────────────┐ │
│  │  Application Services Layer (42 servicios)                          │ │
│  │  Usuario(14) | Reserva(9) | Reseña(5) | Presupuesto(3) | Pago(3)  │ │
│  │  Servicio(5) | Notificacion(2) | Consulta(1)                       │ │
│  └──────────────────────────────────┬──────────────────────────────────┘ │
│                                     │                                      │
│  ┌──────────────────────────────────▼──────────────────────────────────┐ │
│  │  Data Access Layer (Repositories + EF Core)                         │ │
│  │  RepositorioUsuario | RepositorioReserva | RepositorioServicio     │ │
│  │  RepositorioReseña  | RepositorioPago    | RepositorioPresupuesto  │ │
│  └──────────────────────────────────┬──────────────────────────────────┘ │
│                                     │                                      │
│  ┌──────────────────────────────────▼──────────────────────────────────┐ │
│  │  Domain Layer (17 entidades + validaciones)                         │ │
│  │  Usuario | Reserva | Servicio | Reseña | Presupuesto | Pago        │ │
│  └──────────────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────┬───────────────────────────────────────┘
                                     │
         ┌───────────────────────────┼───────────────────────────┐
         │                           │                           │
         ↓                           ↓                           ↓
┌──────────────────┐     ┌──────────────────┐      ┌──────────────────┐
│  SQL Server DB   │     │  Azure Blob      │      │  Servicios Ext   │
│  - Usuarios      │     │  - Imágenes Svc  │      │  - MercadoPago   │
│  - Reservas      │     │  - Imágenes Rev  │      │  - OpenAI        │
│  - Servicios     │     │                  │      │  - Resend Email  │
│  - Pagos         │     │                  │      │  - HCaptcha      │
└──────────────────┘     └──────────────────┘      └──────────────────┘
```

### Descripción de Componentes

#### **Frontend Components**
1. **Módulo Público**: Landing, autenticación, catálogo
2. **Módulo Cliente**: Dashboard, agendamiento, pagos, reseñas
3. **Módulo Admin**: Gestión de reservas, clientes, servicios, moderación
4. **Servicios API**: Capa de integración con backend REST

#### **Backend Components**
1. **Controllers**: Capa de presentación REST
2. **Application Services**: Casos de uso y orquestación
3. **Repositories**: Abstracción de acceso a datos
4. **Domain Entities**: Modelos de negocio con validaciones

#### **External Services**
1. **Azure Blob Storage**: Almacenamiento de imágenes
2. **MercadoPago API**: Procesamiento de pagos
3. **OpenAI API**: Moderación de contenido
4. **Resend API**: Envío de emails transaccionales
5. **HCaptcha**: Protección contra bots

---

## Diagrama de Despliegue

```
┌────────────────────────────────────────────────────────────────────────────┐
│                          ENTORNO DE PRODUCCIÓN                             │
└────────────────────────────────────────────────────────────────────────────┘

                              ┌─────────────────┐
                              │  Internet       │
                              │  Users          │
                              └────────┬────────┘
                                       │ HTTPS
                              ┌────────▼────────┐
                              │  Azure Front    │
                              │  Door / CDN     │
                              └────────┬────────┘
                                       │
                 ┌─────────────────────┼─────────────────────┐
                 │                     │                     │
                 ↓                     ↓                     ↓
┌────────────────────────┐  ┌────────────────────────┐  ┌────────────────────┐
│  Azure Static          │  │  Azure App Service     │  │  Azure Storage     │
│  Web Apps              │  │  (.NET 8 Runtime)      │  │  (Blob)            │
│  ┌──────────────────┐  │  │  ┌──────────────────┐  │  │  ┌──────────────┐ │
│  │  React Frontend  │  │  │  │  Backend API     │  │  │  │  Images      │ │
│  │  - Build Output  │  │  │  │  - Controllers   │  │  │  │  - Servicios │ │
│  │  - Static Files  │  │  │  │  - Services      │  │  │  │  - Reseñas   │ │
│  │  - SPA Routing   │  │  │  │  - Repositories  │  │  │  │              │ │
│  └──────────────────┘  │  │  └──────────────────┘  │  │  └──────────────┘ │
│  Port: 443             │  │  Port: 443 (HTTPS)     │  │  HTTPS Access     │
└────────────────────────┘  └───────────┬────────────┘  └────────────────────┘
                                        │
                                        │ SQL Protocol (TDS)
                                        │ Encrypted Connection
                                        ↓
                            ┌────────────────────────┐
                            │  Azure SQL Database    │
                            │  ┌──────────────────┐  │
                            │  │  WinniDB         │  │
                            │  │  - Tables        │  │
                            │  │  - Indexes       │  │
                            │  │  - Constraints   │  │
                            │  └──────────────────┘  │
                            │  Auto-backup enabled   │
                            └────────────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│                          SERVICIOS EXTERNOS                                │
└────────────────────────────────────────────────────────────────────────────┘

         ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
         │  MercadoPago     │  │  OpenAI API      │  │  Resend Email    │
         │  - Payment API   │  │  - Moderation    │  │  - SMTP Service  │
         │  - Webhooks      │  │  - Scoring       │  │  - Templates     │
         └──────────────────┘  └──────────────────┘  └──────────────────┘
                ▲                       ▲                     ▲
                │ HTTPS                 │ HTTPS               │ HTTPS
                └───────────────────────┴─────────────────────┘
                           Llamadas desde App Service
```

### Características del Despliegue

#### **Frontend - Azure Static Web Apps**
- **Hosting**: Sitio estático con React SPA
- **CDN**: Distribución global con baja latencia
- **HTTPS**: Certificado SSL automático
- **Routing**: Fallback a index.html para SPA routing
- **Variables**: VITE_API_URL, VITE_MP_PUBLIC_KEY

#### **Backend - Azure App Service**
- **Runtime**: .NET 8.0 LTS
- **Scaling**: Auto-scaling basado en carga
- **Logging**: Application Insights integrado
- **Health Checks**: Endpoint de health
- **Connection Strings**: Inyectadas como variables de entorno
- **Variables de Entorno**: JWT_KEY, MercadoPago tokens, OpenAI key

#### **Base de Datos - Azure SQL Database**
- **Tier**: Standard/Premium según carga
- **Backup**: Backups automáticos diarios
- **Geo-Replication**: Opcional para HA
- **Firewall**: Solo acepta conexiones desde App Service
- **Encryption**: TDE (Transparent Data Encryption)

#### **Almacenamiento - Azure Blob Storage**
- **Container**: Público para imágenes de servicios y reseñas
- **CDN**: Integración opcional para delivery
- **Lifecycle**: Políticas de archivado automático

#### **Monitoreo**
- **Application Insights**: Logs, traces, métricas
- **Azure Monitor**: Alertas de disponibilidad y performance
- **Log Analytics**: Queries centralizadas de logs


## Integraciones Externas

### 1. **Azure Blob Storage**

**Propósito**: Almacenamiento de imágenes de servicios y reseñas

**Integración**:
```
Cliente → Backend API → ServicioImagenes → Azure Blob Client → Azure Storage
```

**Operaciones**:
- Upload de imágenes desde formularios
- Generación de URLs públicas
- Eliminación de imágenes (soft delete)

**Configuración**:
- Connection String en variables de entorno
- Container público: `winni-images`
- Nombres de blob con GUID

---

### 2. **MercadoPago**

**Propósito**: Procesamiento de pagos de presupuestos

**Flujo de Pago**:
```
1. Cliente → Frontend: Solicita pagar presupuesto
2. Frontend → Backend API: POST /pago/crear
3. Backend → MercadoPago API: Crear Preference
4. MercadoPago → Backend: Devuelve Preference ID
5. Backend → Frontend: Preference ID
6. Frontend → MercadoPago Checkout: Redirect con Preference ID
7. Usuario → MercadoPago: Completa pago
8. MercadoPago → Backend Webhook: POST /webhook (notificación)
9. Backend → MercadoPago API: Verificar estado del pago
10. Backend → DB: Actualizar estado de pago
```

**Endpoints Utilizados**:
- `POST /v1/payments`: Crear preferencia de pago
- `GET /v1/payments/{id}`: Consultar estado

**Webhooks**:
- URL: `https://api.winni.com/mercadopago/webhook`
- Validación: HMAC signature con secret
- Procesamiento idempotente

**Configuración**:
- Public Key: En frontend (VITE_MP_PUBLIC_KEY)
- Access Token: En backend (variable de entorno)

---

### 3. **OpenAI API**

**Propósito**: Moderación automática y scoring de reseñas

**Servicios**:

**a) Moderación de Contenido**:
```
Usuario → Crea reseña → Backend recibe
    → ModeracionOpenAi.Moderar(descripcion)
    → OpenAI API: POST /v1/moderations
    → Respuesta: {flagged: true/false, categories: [...]}
    → Si flagged: Estado = Desaprobada
    → Si no flagged: Estado = Aprobada
```

**b) Evaluación de Puntuación**:
```
Reseña aprobada → EvaluarPuntajeResenia.Ejecutar(descripcion, puntuacion)
    → OpenAI API: POST /v1/chat/completions (GPT-4)
    → Prompt: "¿La descripción coincide con la puntuación?"
    → Respuesta: Análisis de coherencia
    → Log para revisión admin
```

**Configuración**:
- API Key en variable de entorno
- Modelo: `gpt-4` o `gpt-3.5-turbo`
- Max Tokens: 150

---

### 4. **Resend Email API**

**Propósito**: Envío de emails transaccionales

**Casos de Uso**:
- Confirmación de registro
- Recuperación de contraseña (con OneTimeToken)
- Confirmación de reserva
- Recordatorios estacionales (mantenimiento)
- Notificaciones admin

**Flujo**:
```
Evento en Backend → EnviarEmail.Ejecutar(destinatario, asunto, contenido)
    → Resend API: POST /emails
    → Respuesta: Email ID
    → Backend guarda Notificacion en DB
```

**Configuración**:
- API Key en variable de entorno
- From: `noreply@winnielectricidad.com`
- Templates: HTML en código

---

### 5. **HCaptcha**

**Propósito**: Protección contra bots en registro

**Flujo**:
```
1. Frontend: Usuario completa formulario de registro
2. Frontend: HCaptcha widget genera token
3. Frontend → Backend: POST /usuario/registro + captchaToken
4. Backend → HCaptcha API: POST /siteverify
5. HCaptcha → Backend: {success: true/false}
6. Si success: Continuar registro
7. Si no success: Rechazar con error 400
```

**Configuración**:
- Site Key: En frontend (público)
- Secret Key: En backend (variable de entorno)

---

## Flujos Principales

### 1. **Flujo de Registro de Cliente**

```
┌──────────┐
│ Usuario  │
└────┬─────┘
     │
     ↓ Accede a /registro
┌─────────────────────────┐
│ Formulario de Registro  │
│ - Datos personales      │
│ - Email y contraseña    │
│ - Dirección             │
│ - HCaptcha              │
└────┬────────────────────┘
     │
     ↓ Completa HCaptcha
┌─────────────────────────┐
│ HCaptcha Verification   │
│ Token generado          │
└────┬────────────────────┘
     │
     ↓ POST /usuario/registro
┌─────────────────────────┐
│ Backend API             │
│ 1. Validar HCaptcha     │
│ 2. Hash de contraseña   │
│ 3. Crear usuario        │
│ 4. Crear dirección      │
│ 5. Guardar en DB        │
└────┬────────────────────┘
     │
     ↓ Usuario creado
┌─────────────────────────┐
│ Redirect a /login       │
│ Mensaje de éxito        │
└─────────────────────────┘
```

---

### 2. **Flujo de Agendamiento de Reserva**

```
┌──────────┐
│ Cliente  │
└────┬─────┘
     │
     ↓ Accede a /cliente/agenda
┌────────────────────────────────┐
│ Página de Agendamiento         │
│ 1. Selecciona servicio(s)      │
│ 2. Selecciona dirección        │
│ 3. Elige tipo (Instalación/M)  │
│ 4. Ingresa descripción         │
└────┬───────────────────────────┘
     │
     ↓ GET /reserva/disponibilidad
┌────────────────────────────────┐
│ Backend calcula horarios       │
│ - Excluye domingos             │
│ - Verifica conflictos          │
│ - Devuelve slots disponibles   │
└────┬───────────────────────────┘
     │
     ↓ Muestra calendario
┌────────────────────────────────┐
│ Cliente selecciona fecha/hora  │
└────┬───────────────────────────┘
     │
     ↓ POST /reserva/crear
┌────────────────────────────────┐
│ Backend valida:                │
│ - 48h anticipación ✓           │
│ - Máximo 30 días ✓             │
│ - No domingo ✓                 │
│ - No conflicto ✓               │
│ → Crea Reserva (Pendiente)    │
│ → Crea Presupuesto asociado   │
└────┬───────────────────────────┘
     │
     ↓ Reserva creada
┌────────────────────────────────┐
│ Notificación al cliente        │
│ Email de confirmación          │
│ Estado: Pendiente aprobación   │
└────────────────────────────────┘
```

---

### 3. **Flujo de Aprobación de Reserva (Admin)**

```
┌──────────┐
│  Admin   │
└────┬─────┘
     │
     ↓ Accede a /admin/panel-reservas
┌────────────────────────────────┐
│ Calendario Semanal             │
│ - Vista de todas las reservas  │
│ - Filtros por estado           │
│ - Código de colores            │
└────┬───────────────────────────┘
     │
     ↓ Selecciona reserva pendiente
┌────────────────────────────────┐
│ Modal con detalles:            │
│ - Cliente                      │
│ - Servicios solicitados        │
│ - Dirección                    │
│ - Fecha y hora                 │
│ - Descripción                  │
│ - Presupuesto asociado         │
└────┬───────────────────────────┘
     │
     ↓ Click en "Aprobar"
┌────────────────────────────────┐
│ POST /reserva/aprobar          │
│ Backend:                       │
│ - Valida estado actual         │
│ - Actualiza a "Confirmada"     │
│ - Envía email al cliente       │
└────┬───────────────────────────┘
     │
     ↓ Reserva confirmada
┌────────────────────────────────┐
│ Calendario se actualiza        │
│ Cliente recibe email           │
│ Reserva lista para servicio    │
└────────────────────────────────┘
```

---

### 4. **Flujo de Pago con MercadoPago**

```
┌──────────┐
│ Cliente  │
└────┬─────┘
     │
     ↓ Accede a /cliente/pago/crear
┌────────────────────────────────┐
│ Página de Pago                 │
│ - Carousel de presupuestos     │
│ - Selecciona presupuesto       │
│ - Ingresa monto                │
└────┬───────────────────────────┘
     │
     ↓ Click en "Pagar"
┌────────────────────────────────┐
│ POST /pago/crear               │
│ Backend:                       │
│ - Valida presupuesto           │
│ - Crea Preference en MP        │
│ - Devuelve Preference ID       │
└────┬───────────────────────────┘
     │
     ↓ Preference ID recibido
┌────────────────────────────────┐
│ Frontend abre MercadoPago      │
│ Wallet con Preference ID       │
└────┬───────────────────────────┘
     │
     ↓ Cliente completa pago en MP
┌────────────────────────────────┐
│ MercadoPago procesa pago       │
│ - Aprobado / Rechazado         │
└────┬───────────────────────────┘
     │
     ↓ Webhook notification
┌────────────────────────────────┐
│ POST /mercadopago/webhook      │
│ Backend:                       │
│ - Valida HMAC signature        │
│ - Consulta estado en MP        │
│ - Actualiza Pago en DB         │
│ - Envía email confirmación     │
└────┬───────────────────────────┘
     │
     ↓ Redirect a resultado
┌────────────────────────────────┐
│ /success o /failure            │
│ Mensaje al usuario             │
└────────────────────────────────┘
```

---

### 5. **Flujo de Creación y Moderación de Reseña**

```
┌──────────┐
│ Cliente  │
└────┬─────┘
     │
     ↓ Accede a /cliente/resenas/crear
┌────────────────────────────────┐
│ Formulario de Reseña           │
│ - Selecciona servicio          │
│ - Puntuación (1-5 estrellas)   │
│ - Descripción                  │
│ - Imagen (opcional)            │
└────┬───────────────────────────┘
     │
     ↓ POST /resena/agregar
┌────────────────────────────────┐
│ Backend:                       │
│ 1. Sube imagen a Azure Blob    │
│ 2. Crea entidad Reseña         │
│ 3. Llama a ModeracionOpenAi    │
└────┬───────────────────────────┘
     │
     ↓ OpenAI Moderation API
┌────────────────────────────────┐
│ POST /v1/moderations           │
│ {                              │
│   "input": "descripcion"       │
│ }                              │
│ → {flagged: false/true}        │
└────┬───────────────────────────┘
     │
     ├─ Si flagged = false
     │  ↓
     │  ┌──────────────────────────┐
     │  │ Estado: Aprobada         │
     │  │ Visible públicamente     │
     │  └──────────────────────────┘
     │
     └─ Si flagged = true
        ↓
        ┌──────────────────────────┐
        │ Estado: Desaprobada      │
        │ No visible               │
        │ Admin puede revisar      │
        └──────────────────────────┘
```

---

## Stack Tecnológico

### Backend
| Componente | Tecnología | Versión |
|-----------|-----------|---------|
| **Framework** | .NET | 8.0 LTS |
| **Lenguaje** | C# | 12 |
| **ORM** | Entity Framework Core | 8.0.0 |
| **Base de Datos** | SQL Server | 2019+ |
| **Autenticación** | JWT Bearer | ASP.NET Core 8.0 |
| **Documentación** | Swagger/Swashbuckle | 6.6.2 |
| **Storage** | Azure.Storage.Blobs | 12.26.0 |
| **Email** | Resend | 0.1.7 |
| **Payments** | MercadoPago SDK | 2.11.0 |
| **IA** | OpenAI | 2.8.0 |

### Frontend
| Componente | Tecnología | Versión |
|-----------|-----------|---------|
| **Framework** | React | 19.1.1 |
| **Routing** | React Router DOM | 7.9.5 |
| **Build Tool** | Vite | 7.1.7 |
| **UI Library** | Material-UI | 7.3.4 |
| **Styling** | Emotion (CSS-in-JS) | 11.14.0 |
| **Date Utils** | date-fns | 2.30.0 |
| **Data Grid** | MUI X Data Grid | 7.29.9 |
| **Payments** | MercadoPago React SDK | 1.0.7 |
| **Captcha** | HCaptcha React | 1.14.0 |
| **Linting** | ESLint | 9.36.0 |

### DevOps & Deployment
| Componente | Tecnología |
|-----------|-----------|
| **Version Control** | Git / GitHub |
| **CI/CD** | GitHub Actions |
| **Frontend Hosting** | Azure Static Web Apps |
| **Backend Hosting** | Azure App Service |
| **Database** | Azure SQL Database |
| **Storage** | Azure Blob Storage |
| **Monitoring** | Application Insights |
| **CDN** | Azure CDN |

### Testing
| Tipo | Framework |
|------|-----------|
| **Unit Tests** | xUnit |
| **Mocking** | Moq |
| **Integration Tests** | WebApplicationFactory |
| **E2E Tests** | xUnit + TestServer |

---

## Consideraciones de Seguridad

1. **Autenticación y Autorización**
   - JWT con expiración de tokens
   - Roles diferenciados (Cliente/Admin)
   - Passwords hasheados con BCrypt

2. **Protección de API**
   - CORS configurado
   - HTTPS obligatorio en producción
   - Rate limiting (pendiente implementación)

3. **Validación de Datos**
   - Validación en múltiples capas (DTO, Entity, DB)
   - Sanitización de inputs
   - HCaptcha en registro

4. **Protección de Datos Sensibles**
   - Variables de entorno para secrets
   - Connection strings encriptadas
   - No se exponen datos sensibles en DTOs

5. **Webhooks**
   - Validación de firma HMAC (MercadoPago)
   - Procesamiento idempotente

6. **Moderación de Contenido**
   - OpenAI Moderation para reseñas
   - Revisión manual disponible para admins

---

## Escalabilidad y Performance

1. **Backend**
   - Async/Await en todas las operaciones I/O
   - Connection pooling de Entity Framework
   - Índices en campos consultados frecuentemente

2. **Frontend**
   - Code splitting con React Router
   - Lazy loading de componentes
   - Optimización de imágenes desde Azure CDN

3. **Base de Datos**
   - Índices en FK y campos de búsqueda
   - Paginación en listados grandes
   - Query optimization con EF Core

4. **Caching** (futuro)
   - Redis para sesiones
   - CDN para assets estáticos
   - Cache de consultas frecuentes

---

## Conclusión

La arquitectura de Winni Electricidad implementa un diseño moderno y escalable con separación clara de responsabilidades. El backend sigue principios SOLID y patrones de diseño establecidos, mientras que el frontend ofrece una experiencia de usuario fluida y responsiva. La integración con servicios externos (Azure, MercadoPago, OpenAI) permite funcionalidades avanzadas sin aumentar la complejidad del núcleo de la aplicación.

**Fortalezas de la Arquitectura:**
- ✅ Separación clara de capas
- ✅ Patrones de diseño bien implementados
- ✅ Validaciones en múltiples niveles
- ✅ Integración con servicios cloud modernos
- ✅ Seguridad implementada (JWT, HCaptcha, HTTPS)
- ✅ Escalabilidad horizontal posible
- ✅ Testing estructurado (Small/Medium/Large)

**Áreas de Mejora Futuras:**
- Implementación de cache distribuido (Redis)
- Rate limiting en API
- Logging estructurado con Serilog
- Metrics y dashboards de negocio
- Circuit breaker para servicios externos
- Notification system en tiempo real (SignalR)

---

**Documento creado**: Febrero 2026  
**Autoras**: Sofía Villanueva (282442) y Carolina Sosa (323678)  
**Proyecto**: Plataforma Digital para Winni Electricidad  
**Universidad**: Analista en Tecnologías de la Información
