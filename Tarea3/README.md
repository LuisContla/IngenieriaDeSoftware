# ✏️ Tarea 3.- Login/Registro y Conexión con Base de Datos
Tarea 3 correspondiente al curso de Ingeniería de Software. El propósito de esta actividad fue aprender a conectar la aplicación con una base de datos, añadir una interfaz de usuario básica y empaquetar todo en contenedores Docker para que pueda ejecutarse en cualquier computadora.

## 🛠️ ¿Cómo Iniciar las Aplicación?

### Requisitos Previos

1. **Docker**: Asegúrese de tener Docker instalado y funcionando. Puede verificar si Docker está instalado correctamente ejecutando el siguiente comando en su terminal:

   ```bash
   docker --version
   ```

2. **Código Fuente**: Asegúrese de tener acceso a los archivos del proyecto, que deben incluir el `Dockerfile`, `.dockerignore`, el código fuente de la aplicación y cualquier archivo de configuración necesario. Dicos archivos se encuentran listos en la carpeta "Tarea 3".

### Construir la imagen Docker

1. Navegue al directorio donde se encuentran los archivos del proyecto, incluyendo el `Dockerfile`.
   
   ```bash
   cd /ruta/a/el/proyecto
   ```

2. Construya la imagen Docker utilizando el siguiente comando:

   ```bash
   docker build -t nombre-imagen .
   ```

   - **`nombre-imagen`**: El nombre que desea darle a la imagen Docker. Puede usar cualquier nombre descriptivo.
   - Este comando leerá el `Dockerfile`, instalará las dependencias necesarias y construirá la imagen de la aplicación.

### Ejecutar el contenedor Docker

Una vez que la imagen se haya creado correctamente, ejecute un contenedor desde esa imagen con el siguiente comando:

```bash
docker run -d -p 8080:8080 --name nombre-contenedor nombre-imagen
```

- **`nombre-contenedor`**: El nombre que desea asignar al contenedor que se creará.
- **`nombre-imagen`**: El nombre de la imagen que ha creado en el paso anterior.
- **`-p 8080:8080`**: Esto mapea el puerto `8080` del contenedor al puerto `8080` de su máquina local, lo que permite acceder a la aplicación a través de `http://localhost:8080`.

Para este proyecto, mi sugerencia es usar los comandos anteriores de la siguiente manera:

   ```bash
   docker build -t tarea_3 .
   ```

   ```bash
   docker run -d -p 8080:8080 --name tarea_3_contenedor tarea_3
   ```

### 3. Acceder a la Aplicación

Una vez que el contenedor esté corriendo, abra un navegador web y acceda a la aplicación usando la siguiente URL:

```
http://localhost:8080
```

### 4. Ver los Logs del Contenedor (Opcional)

Si desea ver los logs para asegurarse de que la aplicación se esté ejecutando correctamente, puede usar el siguiente comando:

```bash
docker logs nombre-contenedor
```

### 5. Detener y Eliminar el Contenedor (Opcional)

Si desea detener el contenedor y eliminarlo después de haber terminado, puede usar los siguientes comandos:

```bash
docker stop nombre-contenedor
docker rm nombre-contenedor
```

## Problemas Comunes

- Si no puede acceder a `http://localhost:8080`, asegúrese de que Docker esté en ejecución y de que el contenedor esté mapeando correctamente los puertos.
- Si ve un error en los logs relacionado con dependencias, verifique el `Dockerfile` y los archivos de configuración.

## Notas Adicionales

- Asegúrese de que su aplicación esté configurada para escuchar en `0.0.0.0:8080` o `0.0.0.0`, lo que hace que sea accesible desde fuera del contenedor.
- Puede cambiar el puerto de mapeo si ya está en uso, modificando el valor `8080` a otro puerto, tanto en el contenedor como en el navegador.

---

¡Listo! Ahora la aplicación está corriendo en Docker.
