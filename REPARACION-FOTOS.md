# Reparación de fotos — 17/09/2026

Se conservan los siete artículos, todos sus campos, marcas, categorías y las diez filas de imágenes. La comparación antes/después quedó en revision-enlaces/datos-antes.xml y datos-despues.xml dentro del espacio de trabajo de Codex. La base tiene respaldo completo previo a la reparación.

- El enlace Google del iPhone se resuelve al enlace directo sin modificar el registro.
- El enlace de Bravia se resuelve al archivo original del mismo servidor.
- Se reemplazaron solamente tres enlaces externos que ya fallaban: imagen 6 (Apple TV), 10 (Moto G7 Play) y 12 (Play 4). Sus valores anteriores están en el respaldo.
- Fuentes de las fotos sustitutas: https://support.apple.com/en-us/101605 , https://www.techradar.com/reviews/moto-g7-play-review , https://www.playstation.com/en-us/ps4/ .
- El visor abre WebP mediante los códecs de Windows. Se verificó en esta computadora. Para otra PC sin ese códec, se puede usar PNG/JPG o compartir la caché ya convertida a PNG.
- Las fotos de enlaces válidos se conservan en Imagenes/Cache. Compartir la carpeta Imagenes completa mantiene las fotos locales y las copias descargadas.
- Agregar URL comprueba que el enlace se pueda abrir como imagen antes de agregarlo. Una página web genérica no es una imagen; el programa explica el error.

## Conservar los artículos
No ejecutar nuevamente el script inicial ni restaurar una base de ejemplo sobre la base en uso. Compilar o actualizar el programa no necesita recrear la base. Antes de modificar datos en mantenimiento, respaldar la base actual. GitHub comparte código, no los artículos guardados en SQL Server.
