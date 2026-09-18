# Fotos y trabajo en equipo

- Para ver fotos: seleccionar un artículo y pulsar **Ver detalle / fotos**, o hacer doble clic en la fila. **Anterior** y **Siguiente** recorren todas las imágenes.
- Para agregar fotos descargadas: en Agregar o Modificar, pulsar **Agregar fotos desde la PC**. Se pueden seleccionar varias. Se admiten JPG, PNG, GIF y BMP (hasta 15 MB y 40 megapíxeles por archivo).
- Los archivos seleccionados se copian a **Imagenes**, junto a la solución, al guardar el artículo. La foto original de Descargas no se modifica. Cancelar no importa archivos.
- También se pueden agregar enlaces con **Agregar URL**. Deben apuntar directamente a una imagen, no a una búsqueda de Google o una página. Los enlaces externos pueden vencer o bloquear la descarga.
- **Quitar de la lista** desvincula esa foto al guardar. No borra la foto original ni elimina archivos que puedan usar otros artículos.

## Para pasar el trabajo a un compañero

Compartir la solución completa, incluida **Imagenes**. Las referencias locales son relativas: no dependen del nombre de usuario ni de la ubicación de Descargas. Al compilar se copian las fotos existentes junto al ejecutable; para distribuir únicamente el programa, incluir también esa carpeta.

GitHub comparte código y archivos, pero **no sincroniza SQL Server**. Los artículos, marcas, categorías y referencias de imágenes que se crean desde el programa viven en la base local. Para que el compañero vea los mismos datos hay que pasarle también un respaldo o un script de datos actualizado. El SQL original de BaseDatos solo contiene los ejemplos iniciales. No sobrescribir su base sin conservar antes sus datos.

Cada integrante debe ajustar App.config al servidor SQL que usa. Este equipo usa (localdb)\MSSQLLocalDB.

No se publicaron cambios ni fotos automáticamente en GitHub.
