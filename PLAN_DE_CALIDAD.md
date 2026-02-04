# Plan de Calidad - Winni Electricidad

## Ejecución del Plan de Calidad

El plan de calidad se ejecutó respetando los lineamientos definidos en el anteproyecto, con adaptaciones acordes a la evolución real del proyecto y a las restricciones de tiempo de los últimos sprints. El objetivo principal fue asegurar la estabilidad del sistema, priorizando un enfoque balanceado entre pruebas automatizadas, pruebas manuales y control de calidad continuo.

## Documentación del Código

En cuanto a la documentación del código, se incorporaron comentarios XML en la mayoría de los endpoints implementados en los controladores. Esto permitió generar una documentación clara y centralizada de la API, facilitando tanto la comprensión del código como la organización de la colección de Postman utilizada durante el desarrollo y las pruebas.

## Estrategia de Testing

Para el aseguramiento de la calidad del software, se implementó la estrategia de testing basada en la pirámide de pruebas, materializada a través de tres proyectos de tests diferenciados: **Tests Small (unitarios)**, **Tests Medium (integración)** y **Tests Large (end-to-end)**.

### Estructura de Proyectos de Tests

El backend del proyecto cuenta con tres proyectos de testing independientes, cada uno con su propio archivo `.csproj` y configuraciones específicas:

#### 1. WinniElectricidad.Tests.Small (Pruebas Unitarias)
- **Objetivo**: Validar el comportamiento de componentes individuales de forma aislada
- **Cantidad**: 15 archivos de tests
- **Características**:
  - Pruebas de lógica de negocio pura
  - Pruebas de validaciones en entidades
  - Pruebas de casos de uso en la capa de aplicación
  - Sin dependencias de infraestructura (base de datos, servicios externos)
  - Ejecución rápida y sin efectos secundarios

#### 2. WinniElectricidad.Tests.Medium (Pruebas de Integración)
- **Objetivo**: Verificar la correcta comunicación entre módulos y capas
- **Cantidad**: 8 archivos de tests
- **Características**:
  - Pruebas de repositorios con base de datos SQLite en memoria
  - Pruebas de casos de uso que integran múltiples servicios
  - Uso de Moq para mockear dependencias externas cuando es necesario
  - Validación de flujos de datos entre capas de aplicación y acceso a datos

#### 3. WinniElectricidad.Tests.Large (Pruebas End-to-End)
- **Objetivo**: Reproducir escenarios completos de usuario validando el flujo completo del sistema
- **Cantidad**: 6 archivos de tests (agrupados por controlador)
- **Características detalladas**: Ver sección siguiente

### Infraestructura de Tests Large (End-to-End)

Los tests Large representan el nivel más alto de la pirámide de pruebas y son fundamentales para validar el funcionamiento integral del sistema. A continuación se describe en detalle su infraestructura:

#### Componentes Principales

##### 1. ApiTestFactory
Clase que extiende `WebApplicationFactory<Program>` y proporciona un entorno aislado para ejecutar la aplicación completa en memoria durante los tests.

**Responsabilidades:**
- Configurar el entorno de testing mediante variables de entorno
- Cargar configuración desde `appsettings.Testing.json`
- Reemplazar la base de datos real por una instancia SQLite en memoria
- Inyectar implementaciones fake de servicios externos (email, imágenes, moderación de contenido)
- Inicializar y crear la base de datos en memoria al arrancar cada test

**Configuración de Base de Datos:**
- Utiliza SQLite con `DataSource=:memory:` para una base de datos volátil
- La conexión permanece abierta durante toda la ejecución del test
- Al finalizar cada suite de tests, la base de datos se descarta automáticamente
- Garantiza aislamiento total entre diferentes ejecuciones de tests

**Reemplazo de Servicios Externos:**
```csharp
- IModeracionOpenAi → ModeracionOpenAiFake
- IServicioImagenes → ServicioImagenesFake
- IEvaluarPuntajeResenia → EvaluarPuntajeReseniaFake
- IEnviarEmail → EnviarEmailFake
```
Este enfoque permite:
- Ejecutar tests sin dependencias de servicios externos reales
- Evitar costos de APIs externas durante las pruebas
- Controlar el comportamiento de servicios externos (simular éxitos, fallos, etc.)
- Acelerar la ejecución de los tests

##### 2. LargeTestBase
Clase base abstracta de la cual heredan todos los tests E2E.

**Proporciona:**
- Instancia de `ApiTestFactory` configurada y lista para usar
- `HttpClient` pre-configurado para realizar peticiones HTTP a la API
- `JwtHelper` para generar tokens de autenticación válidos o expirados durante los tests
- Método `LimpiarHeaders()` ejecutado antes de cada test para resetear headers de autorización

**Patrón de uso:**
```csharp
public class UsuarioControllerTests : LargeTestBase
{
    [Test]
    public async Task AlgunTest()
    {
        // Arrange: configurar datos y autenticación
        var token = Jwt.GenerarTokenValido(...);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act: ejecutar petición HTTP
        var response = await Client.GetAsync("/endpoint");
        
        // Assert: verificar resultado
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```

##### 3. DbSeeder
Utilidad estática para preparar datos de prueba en la base de datos en memoria.

**Métodos principales:**
- `CleanDatabaseAsync()`: Limpia tablas específicas entre tests
- `SeedServiciosAsync()`: Inserta servicios de prueba
- `SeedUsuarioConDireccionesAsync()`: Crea un usuario cliente con direcciones de prueba
- `SeedReservaAsync()`: Crea una reserva completa con todos sus datos relacionados

**Ventajas:**
- Reutilización de código para preparar escenarios de prueba comunes
- Datos consistentes y predecibles entre tests
- Facilita el mantenimiento de los tests al centralizar la lógica de creación de datos

##### 4. JwtHelper
Generador de tokens JWT para simular autenticación en los tests.

**Capacidades:**
- `GenerarTokenValido()`: Crea tokens válidos con claims personalizados (ID, email, rol)
- `GenerarTokenExpirado()`: Crea tokens caducados para probar validación de expiración
- `GenerarTokenConRol()`: Crea tokens con roles específicos para probar autorización

**Configuración:**
- Lee la clave secreta, issuer y audience desde `appsettings.Testing.json`
- Genera tokens compatibles con la validación real de la aplicación
- Permite especificar tiempo de expiración personalizado

##### 5. Mocks y Fakes
Implementaciones simplificadas de servicios externos utilizados en los tests:

- **ModeracionOpenAiFake**: Siempre retorna `false` (contenido no ofensivo) para evitar llamadas a OpenAI
- **ServicioImagenesFake**: Simula subida de imágenes sin interactuar con sistemas de almacenamiento real
- **EvaluarPuntajeReseniaFake**: Proporciona evaluaciones predefinidas de reseñas
- **EnviarEmailFake**: Simula envío de correos sin SMTP real

#### Organización de Tests por Controlador

Los tests Large están organizados en carpetas por controlador/módulo:
- `Usuario/`: Tests de endpoints de usuario (direcciones, perfil, etc.)
- `Servicio/`: Tests de CRUD y búsqueda de servicios
- `Reserva/`: Tests de creación, modificación y cancelación de reservas
- `Presupuesto/`: Tests de generación y gestión de presupuestos
- `Notificacion/`: Tests de sistema de notificaciones
- `Reseña/`: Tests de creación y moderación de reseñas

#### Tecnologías y Herramientas

- **Framework de testing**: NUnit 3.14.0
- **HTTP Testing**: Microsoft.AspNetCore.Mvc.Testing 8.0.0
- **Base de datos en memoria**: Microsoft.Data.Sqlite 8.0.0 + Microsoft.EntityFrameworkCore.Sqlite 8.0.0
- **Cobertura de código**: Coverlet (msbuild y collector) 6.0.x
- **Target Framework**: .NET 8.0

#### Ventajas de esta Arquitectura

1. **Aislamiento total**: Cada test se ejecuta en un contexto limpio con su propia base de datos en memoria
2. **Independencia de servicios externos**: Los fakes eliminan dependencias externas y costos asociados
3. **Reproducibilidad**: Los tests producen resultados consistentes independientemente del entorno
4. **Velocidad**: SQLite en memoria es extremadamente rápido comparado con una base de datos real
5. **Facilidad de debugging**: Se puede ejecutar la API completa en modo debug durante los tests
6. **Flexibilidad**: Permite probar tanto casos exitosos como escenarios de error de forma controlada

## Adaptaciones al Plan Original

Si bien el plan original proponía una mayor base de pruebas unitarias, durante los últimos sprints se priorizó un enfoque más equilibrado entre pruebas automatizadas y pruebas manuales, manteniendo una cantidad similar de tests small y large. Esta decisión se tomó con el objetivo de asegurar al menos un test que validara el flujo completo del sistema, sin comprometer el avance funcional del proyecto en la etapa final, donde se incorporaron varias funcionalidades nuevas. La reducción de profundidad en algunos tests automatizados fue compensada con pruebas manuales.

## Integración Continua

Las pruebas automatizadas se ejecutaron mediante GitHub Actions (GHA) en cada Pull Request creado. Ante el fallo de cualquier test, el proceso de integración quedaba bloqueado, impidiendo el merge hasta la corrección del error, garantizando la estabilidad de la rama de develop.

El workflow de CI ejecuta:
1. Compilación del proyecto
2. Ejecución de tests Small
3. Ejecución de tests Medium
4. Ejecución de tests Large
5. Generación de reporte de cobertura

## Pruebas Manuales

Además de las pruebas automatizadas, se realizaron pruebas manuales tanto en el entorno local como en el entorno de desarrollo, verificando el cumplimiento de los criterios de aceptación definidos para cada historia de usuario. Estas pruebas no fueron documentadas formalmente en TestRail, sino que se gestionaron como parte del proceso de validación de cada historia, considerándose cumplida una tarea únicamente cuando los criterios de aceptación eran satisfechos.

## Análisis Estático de Código

Como complemento al proceso de calidad, se integró SonarCloud, el cual se ejecutó automáticamente sobre la rama develop en cada Pull Request. Esta herramienta permitió analizar métricas de calidad del código, incluyendo:
- Cobertura de tests
- Duplicación de código
- Vulnerabilidades de seguridad
- Code smells
- Complejidad ciclomática

## Pruebas de Aceptación de Usuario (UAT)

Finalmente, se llevó a cabo una instancia de pruebas de aceptación de usuario (UAT) durante el Sprint 7. En esta etapa, el cliente validó el funcionamiento general del sistema y propuso ajustes, los cuales fueron incorporados y corregidos dentro del mismo sprint.

---

## Comparación con el Plan Original

### Plan Original Propuesto

El plan de calidad original contemplaba:
- Cumplimiento de requerimientos y objetivos del cliente
- Protección de datos sensibles de forma segura
- Código estructurado, eficiente, legible y mantenible
- Documentación clara y completa siguiendo estándares organizacionales (formatos 302, 303, 306)
- Pirámide de tests con amplia base de pruebas unitarias
- Tres soluciones de tests (small, medium, large)
- Ejecución automática mediante GitHub Actions con bloqueo de merge ante fallos
- Pruebas manuales documentadas en TestRail
- Integración con SonarCloud con umbral de cobertura del 80% (warning, no bloqueante)
- Instancia final de UAT con confirmación escrita del cliente

### Implementación Real

La implementación mantuvo la mayoría de los lineamientos con las siguientes adaptaciones:

**Mantuvido sin cambios:**
- ✅ Estructura de tres proyectos de tests (Small, Medium, Large)
- ✅ Ejecución automática en GitHub Actions con bloqueo de merge
- ✅ Integración con SonarCloud para análisis de calidad
- ✅ Documentación con comentarios XML en controllers
- ✅ Instancia de UAT con el cliente en Sprint 7

**Adaptado:**
- 📊 Distribución de tests más balanceada que la pirámide estricta original (15 Small, 8 Medium, 6 Large con múltiples tests cada uno)
- 🔄 Documentación de pruebas manuales gestionada informalmente en lugar de TestRail
- ⚖️ Mayor énfasis en pruebas E2E para garantizar flujos completos ante restricciones de tiempo

**Justificación de las adaptaciones:**
Las modificaciones realizadas respondieron a restricciones de tiempo en los últimos sprints y a la necesidad de garantizar la estabilidad del sistema mediante pruebas end-to-end que validaran flujos completos. La infraestructura de tests Large desarrollada (con ApiTestFactory, LargeTestBase, Mocks, DbSeeder y JwtHelper) proporciona una base sólida y reutilizable que compensa la menor cantidad de tests unitarios y facilita la expansión futura de la suite de pruebas.
