# 📋 ComunaReclamos - Guía de Uso

## 🚀 Inicio Rápido

### URL de la Aplicación
```
http://localhost:5174
```

---

## 👤 Para Ciudadanos

### 1️⃣ Crear un Reclamo

**URL:** `http://localhost:5174/Reclamos/Crear`

**Pasos:**
1. Selecciona el tipo de problema (Alumbrado, Bacheo, Recolección, Espacios Verdes)
2. Ingresa la ubicación:
   - Usa GPS automático o
   - Escribe la dirección manualmente
3. Escribe la descripción (mínimo 10 caracteres)
4. Adjunta fotos (opcional, máximo 3)
5. Ingresa tu nombre y teléfono
6. Presiona "Enviar reclamo"

### 2️⃣ Ver Confirmación

**Automático después de enviar:**
- ✓ Verás mensaje de éxito
- ✓ Tu código de seguimiento (ej: ABCD-1234)
- ✓ Puedes copiar el código
- ✓ Ver detalles del reclamo
- ✓ Estado del reclamo (progreso)

### 3️⃣ Seguir tu Reclamo

**URL:** `http://localhost:5174/Reclamos/Seguir`

**Cómo:**
1. Escribe tu código de seguimiento
2. Presiona "Buscar"
3. Verás:
   - Estado actual (Recibido, En revisión, En curso, Resuelto)
   - Datos del reclamo
   - Historial de actualizaciones
   - Comentarios de la comuna

---

## 🔐 Para Administradores

### Login Admin

**URL:** `http://localhost:5174/Admin/Login`

**Credenciales por defecto:**
```
Usuario: admin
Contraseña: Admin123!
```

### Cambiar Credenciales (Opcional)

Edita el archivo: `ComunaReclamos.Web/appsettings.json`

```json
{
  "Admin": {
	"Username": "tu_usuario",
	"Password": "tu_contraseña"
  }
}
```

Luego reinicia la aplicación.

### 2️⃣ Panel de Control

**URL:** `http://localhost:5174/Admin` (después de login)

#### Ver Reclamos

**Dashboard:**
- 📊 Resumen: Nuevos, En curso, Resueltos este mes
- 🔍 Filtros: Todos, Nuevos, En curso, Resueltos

**Por cada reclamo ves:**
- Código de seguimiento
- Categoría
- Ubicación
- Vecino (nombre + teléfono)
- Descripción
- **Fotos adjuntas** (clickeables para ver en grande)
- Historial completo de cambios

#### Editar Reclamo

**Acciones disponibles:**
1. **Cambiar Estado:**
   - Recibido
   - En revisión
   - En curso
   - Resuelto
   - Rechazado

2. **Asignar a Cuadrilla:**
   - Selecciona una cuadrilla
   - Se notificará automáticamente

3. **Agregar Notas Internas:**
   - Solo visible para administradores
   - Máximo 1000 caracteres

4. **Agregar Comentario Público:**
   - Visible para el vecino en "Seguimiento"
   - Máximo 500 caracteres
   - Requerido si cambias a "Resuelto"

#### Ver Fotos

- Click en la foto = Abre en tamaño completo
- Nueva pestaña = Puedes descargar
- PNG, JPG, GIF soportados

### 3️⃣ Logout

- Botón "Salir" en la esquina superior derecha

---

## 📱 Estados del Reclamo

```
FLUJO NORMAL:
Recibido → En revisión → En curso → Resuelto

FLUJO RECHAZADO:
Recibido → En revisión → Rechazado

Estados disponibles:
- 🟡 Recibido: Reclamo ingresado al sistema
- 🔵 En revisión: Siendo analizado
- 🔴 En curso: En proceso de solución
- 🟢 Resuelto: Problema solucionado
- ⚫ Rechazado: No procedente
```

---

## 📊 Validaciones

### Campos Requeridos
- ✓ Tipo de problema
- ✓ Dirección (5-500 caracteres)
- ✓ Descripción (10-2000 caracteres)
- ✓ Nombre del vecino (2-200 caracteres)
- ✓ Teléfono (10-20 caracteres, formato: 223-1234567)

### Fotos
- ✓ Máximo 3 fotos
- ✓ Máximo 5MB por foto
- ✓ Formatos: JPG, PNG, GIF
- ✓ Son opcionales

### Mensajes de Validación
- En cliente (instantáneo)
- En servidor (al enviar)
- Mensajes claros en español

---

## 🛠️ Solución de Problemas

### "No veo reclamos en el admin"
**Solución:**
1. Verifica que hayas iniciado sesión (botón "Salir" visible)
2. Crea un reclamo de prueba desde `/Reclamos/Crear`
3. El reclamo debe aparecer inmediatamente

### "Error de conexión a BD"
**Solución:**
1. Verifica que SQL Server Express está corriendo
2. Revisa la cadena de conexión en `appsettings.json`
3. Reinicia la aplicación

### "Las fotos no se ven"
**Solución:**
1. Verifica que `/uploads/reclamos/` existe
2. Las fotos deben tener permisos de lectura
3. Intenta F5 (refrescar página)

### "No recuerdo mi contraseña admin"
**Solución:**
1. Edita `appsettings.json`
2. Cambia la contraseña
3. Reinicia la aplicación

---

## 🔒 Seguridad

- Contraseña hasheada con SHA256
- Comparación en tiempo constante
- Cookies seguras (HttpOnly, Secure, SameSite)
- HTTPS habilitado en producción
- Validación en cliente y servidor

---

## 📁 Estructura de Archivos

```
ComunaReclamos/
├── ComunaReclamos.Data/
│   ├── Entities/
│   │   ├── Reclamo.cs
│   │   ├── FotoReclamo.cs
│   │   └── HistorialReclamo.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   └── Migrations/
│
├── ComunaReclamos.Web/
│   ├── Pages/
│   │   ├── Reclamos/
│   │   │   ├── Crear.cshtml
│   │   │   ├── Confirmacion.cshtml
│   │   │   ├── Seguir.cshtml
│   │   │   └── *.cshtml.cs
│   │   ├── Admin/
│   │   │   ├── Index.cshtml
│   │   │   ├── Login.cshtml
│   │   │   └── *.cshtml.cs
│   │   └── Index.cshtml
│   ├── wwwroot/
│   │   ├── css/site.css
│   │   └── uploads/reclamos/ (fotos)
│   ├── appsettings.json
│   └── Program.cs
```

---

## 📚 API Endpoints (Internos)

### Crear Reclamo
```
POST /Reclamos/Crear
Content-Type: multipart/form-data
```

### Buscar por Código
```
GET /Reclamos/Seguir?codigo=XXXX-XXXX
```

### Admin
```
GET  /Admin                    (Lista reclamos)
GET  /Admin?id=1               (Ver detalle)
GET  /Admin?estado=Recibido    (Filtrar por estado)
POST /Admin?handler=Actualizar (Actualizar reclamo)
POST /Admin?handler=Salir      (Logout)
```

---

## 🐛 Reportar Errores

Si encuentras un error:
1. Revisa la consola (F12 en navegador)
2. Revisa los logs de la aplicación
3. Anota el código de seguimiento si aplica
4. Intenta replicar el error

---

## ✨ Próximas Mejoras

- [ ] Notificaciones por email
- [ ] Búsqueda avanzada
- [ ] Exportar a PDF
- [ ] Gráficos de estadísticas
- [ ] Integración con Google Maps
- [ ] API REST pública
- [ ] Autenticación 2FA
- [ ] Auditoría de cambios

---

## 📞 Soporte

Para problemas técnicos:
1. Compilar: `dotnet build`
2. Ejecutar: `dotnet run --project ComunaReclamos.Web`
3. Revisar logs en consola
4. Verificar base de datos: `dotnet ef database update`

---

**Versión:** 1.0.0  
**Última actualización:** 13/11/2026  
**Estado:** ✅ Listo para producción
