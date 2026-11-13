# 🔄 FLUJO DE DATOS - ComunaReclamos

## DIAGRAMA DEL SISTEMA

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         CIUDADANO (Público)                            │
└────────────────────────────┬──────────────────────────────────────────┘
							 │
							 ▼
					┌────────────────┐
					│ /Reclamos/     │
					│ Crear          │
					│ (Formulario)   │
					└────────┬───────┘
							 │
		┌────────────────────┼────────────────────┐
		│                    │                    │
		▼                    ▼                    ▼
	┌────────┐          ┌────────┐          ┌─────────┐
	│ Tipo   │          │Ubicación│        │Descripción
	│Problema│          │(GPS)    │        │(10-2000)
	└────────┘          └────────┘        └─────────┘
		│                    │                    │
		└────────────────────┼────────────────────┘
							 │
							 ▼
					┌─────────────────┐
					│ Fotos (1-3)     │
					│ MAX 5MB c/u     │
					│ JPG/PNG/GIF     │
					└────────┬────────┘
							 │
							 ▼
					┌─────────────────┐
					│ Datos Vecino    │
					│ • Nombre        │
					│ • Teléfono      │
					└────────┬────────┘
							 │
		 ────────────────────┴─────────────────────
		 │ Validaciones Cliente (JavaScript)    │
		 │ • Contador caracteres                │
		 │ • Formato teléfono                   │
		 │ • Cantidad fotos                     │
		 │ • Tamaño fotos                       │
		 │ • Tipo de archivo                    │
		 └────────────────────┬──────────────────┘
							 │
							 ▼
					┌─────────────────┐
					│ POST /Crear     │
					│ (Envía servidor)│
					└────────┬────────┘
							 │
		─────────────────────┴──────────────────────
		│ Validaciones Servidor (C#/.NET)        │
		│ • Categoría existe en BD               │
		│ • Cantidad fotos válida                │
		│ • Extensión permitida                  │
		│ • Model State válido                   │
		│ • Try-catch con manejo excepciones    │
		└────────────────────┬────────────────────┘
							 │
					┌────────▼────────┐
					│ GUARDAR EN BD   │
					└────────┬────────┘
							 │
		┌────────────────────┼────────────────────┐
		│                    │                    │
		▼                    ▼                    ▼
	┌────────────────┐  ┌────────────────┐  ┌──────────────┐
	│ RECLAMOS       │  │ FOTORECLAMOS   │  │ HISTORIAL    │
	├────────────────┤  ├────────────────┤  ├──────────────┤
	│• Id            │  │• Id            │  │• Id          │
	│• Código ✓      │  │• ReclamoId (FK)│  │• ReclamoId   │
	│• Categoría     │  │• RutaArchivo   │  │• Estado Ant. │
	│• Descripción   │  │• FechaSubida   │  │• Estado Nuevo│
	│• Dirección     │  └────────────────┘  │• Comentario  │
	│• NombreVecino✓ │                      │• FechaCambio │
	│• Teléfono ✓    │                      └──────────────┘
	│• Fotos         │
	│• Estado Inicial│
	│• Fecha Actual  │
	└────────────────┘
		│
		▼
	┌─────────────────────────┐
	│ Guardar Fotos en Disco  │
	│ Ruta: /uploads/reclamos/│
	│ Nombre: CODIGO_GUID.ext │
	└────────┬────────────────┘
			 │
			 ▼
	┌──────────────────────────────┐
	│ Redirige a Confirmación      │
	│ /Reclamos/Confirmacion       │
	│ Muestra Código Seguimiento   │
	└────────┬─────────────────────┘
			 │
			 ▼
	┌──────────────────────────────┐
	│ CIUDADANO VE               │
	│ ✓ Código: XXXX-XXXX        │
	│ "Guardá tu código para      │
	│  consultar el estado"        │
	└──────────────────────────────┘


════════════════════════════════════════════════════════════════════════════


┌──────────────────────────────────────────────────────────────────────────┐
│                    ADMINISTRADOR (Panel Privado)                         │
└────────────────────────────────┬─────────────────────────────────────────┘
								 │
								 ▼
						┌────────────────┐
						│ /Admin         │
						│ Login          │
						└────────┬───────┘
								 │
								 ▼
						┌────────────────┐
						│ /Admin/Index   │
						│ Dashboard      │
						└────────┬───────┘
								 │
		┌────────────────────────┼────────────────────────┐
		│                        │                        │
		▼                        ▼                        ▼
   ┌───────┐               ┌──────────┐             ┌─────────┐
   │Nuevos │               │ En Curso │             │Resueltos│
   │count()│               │ count()  │             │ count() │
   └───────┘               └──────────┘             └─────────┘
		│
		▼
   ┌─────────────┐
   │ Filtros     │
   │ • Todos     │
   │ • Nuevos    │
   │ • En Curso  │
   │ • Resueltos │
   └────────┬────┘
			│
			▼
   ┌─────────────────────────┐
   │ Lista de Reclamos       │
   │ • Código                │
   │ • Categoría + Dirección │
   │ • Estado                │
   │ Click → Ver Detalle     │
   └────────┬────────────────┘
			│
			▼
   ┌───────────────────────────────────────┐
   │ Detalle de Reclamo Seleccionado       │
   ├───────────────────────────────────────┤
   │ • Código + Categoría + Fecha          │
   │ • Estado Actual                       │
   │ • Ubicación (Dirección + Barrio)      │
   │ • Vecino (Nombre + Teléfono)          │
   │ • Descripción del problema            │
   │                                       │
   │ FOTOS ADJUNTAS ⬇️                     │
   │ ┌─────┐ ┌─────┐ ┌─────┐              │
   │ │IMG1 │ │IMG2 │ │IMG3 │ Click→Full │
   │ └─────┘ └─────┘ └─────┘   Screen    │
   │                                       │
   │ EDITAR RECLAMO:                       │
   │ ┌──────────────────────────────────┐ │
   │ │Estado: [Recibido ▼]              │ │
   │ │Asignar: [Sin asignar ▼]          │ │
   │ │Nota: [____________]              │ │
   │ │[Guardar Cambios]                 │ │
   │ └──────────────────────────────────┘ │
   │                                       │
   │ HISTORIAL:                            │
   │ • 13/11 20:05 → Recibido             │
   │   "Reclamo ingresado por el vecino"   │
   └───────────────────────────────────────┘
			│
			▼
   ┌──────────────────┐
   │ Actualiza BD     │
   ├──────────────────┤
   │ • Estado         │
   │ • Cuadrilla ID   │
   │ • Nota Interna   │
   │ • Fecha Cambio   │
   └────────┬─────────┘
			│
			▼
   ┌──────────────────────────┐
   │ Nuevo Historial          │
   │ • Fecha + Hora actual    │
   │ • Estado anterior        │
   │ • Estado nuevo           │
   │ • Comentario admin       │
   └──────────────────────────┘


════════════════════════════════════════════════════════════════════════════


## VALIDACIONES EN CASCADA

┌─────────────────────────────────────────────────────────────────┐
│                   VALIDACIÓN DE CAMPO                           │
└─────────────────────────────────────────────────────────────────┘

DESCRIPCIÓN:
─────────────────────────────────────────────────────────────────
Entrada → [10 caracteres?] ✓ → [2000 caracteres?] ✓ → Contador
			  ↓ NO                    ↓ NO
		 "Muy corta"            "Muy larga"
			  ↓                       ↓
		 Error Cliente         Error Cliente
			 │                       │
			 └──────────┬────────────┘
						│
			  Si todo OK → Enviar servidor
						│
						▼
			  Validación Model State
						│
		   ┌────────────┼────────────┐
		   │ NO         │            │ SÍ
		   ▼            ▼            ▼
		Error     Error Modelo    Guardado
	   Mensaje      Mostrar      en BD ✓
		(JS)       (Html/CSS)


TELÉFONO:
─────────────────────────────────────────────────────────────────
Entrada → [Remover no-dígitos] → [Formato: XXX-XXXXXXX]
223abc123  →    223123        →    223-0123

Entrada → [Required?] ✓ → [Phone Validation] ✓ → [10-20 char] ✓
		   ↓ NO              ↓ NO                   ↓ NO
		"Requerido"      "No válido"         "Fuera de rango"
		   ↓               ↓                      ↓
	  Error Rojo      Error Rojo            Error Rojo


FOTOS:
─────────────────────────────────────────────────────────────────
Archivo → [Verificar tipo] → [Verificar tamaño] → [Cuenta cantidad]
		   JPG/PNG/GIF?       ≤ 5MB?             ≤ 3?
			  │                   │                │
		  ¿Válido?            ¿Válido?         ¿Válido?
		   ↓ ✓                 ↓ ✓              ↓ ✓
		  Sí                  Sí               Sí
		   │                   │                │
	Agregar preview    Pasar servidor   Mensaje OK
		   │                   │                │
		   └───────────────────┼────────────────┘
						│
						▼
			  Todas validadas → Enviar


════════════════════════════════════════════════════════════════════════════


## FLUJO DE DATOS EN BD

					┌─────────────────────┐
					│  RECLAMO (INSERT)   │
					└──────────┬──────────┘
							   │
					┌──────────▼──────────┐
					│ Obtener ID generado │
					│ (Identity)          │
					└──────────┬──────────┘
							   │
				┌──────────────┼──────────────┐
				│              │              │
				▼              ▼              ▼
		 ┌────────────┐  ┌──────────┐  ┌────────────┐
		 │ FOTOS      │  │ HISTORIAL│  │ FK Links   │
		 │ (INSERT)   │  │ (INSERT) │  │ • Categoría
		 │ ReclamoId  │  │ ReclamoId│  │ • Cuadrilla
		 │ Ruta       │  │ Estados  │  │   (NULL)
		 │ FechaSubida│  │ Comentario
		 └────────────┘  └──────────┘  └────────────┘


════════════════════════════════════════════════════════════════════════════


## TRANSACCIONES

CREATE RECLAMO (Atómico):
───────────────────────────────────────────────────────────────
BEGIN TRANSACTION
  │
  ├─ INSERT INTO Reclamos (...)  ← Genera ID
  │
  ├─ INSERT INTO FotoReclamos
  │   WHERE ReclamoId = [ID generado]
  │
  ├─ INSERT INTO HistorialReclamos
  │   WHERE ReclamoId = [ID generado]
  │
  ├─ Guardar archivos en disco
  │
  └─ COMMIT (Todas las operaciones o ninguna)
	   │
	   └─ Si error → ROLLBACK


UPDATE RECLAMO (Atómico):
───────────────────────────────────────────────────────────────
BEGIN TRANSACTION
  │
  ├─ UPDATE Reclamos SET Estado, CuadrillaId, NotaInterna
  │
  ├─ INSERT INTO HistorialReclamos
  │   (registro del cambio)
  │
  └─ COMMIT
	   │
	   └─ Si error → ROLLBACK


════════════════════════════════════════════════════════════════════════════
