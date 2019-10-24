# cryptovote-core
**cryptovote** es una prueba de conceptos sobre como podr&iacute;a ser un sistema de votaci&oacute;n basado en una blockchain p&uacute;blica.

La elecci&oacute;n de una **blockchain p&uacute;blica** es para que todas las personas que quieran participar del procesamiento, validaci&oacute;n y recuento de los votos, puedan hacerlo.

La informaci&oacute;n no es que est&aacute; centralizada con un acceso p&uacute;blico sino que cada **nodo** tiene una copia de toda la informaci&oacute;n y ning&uacute;n nodo tiene mayor relevancia que otro, de ah&iacute; la protecci&oacute;n contra la manipulaci&oacute;n.

## Componentes
* **nodo**: es el programa que mantiene y asegura una copia de la informaci&oacute;n
* **web**: se encarga de exponer la informaci&oacute;n como una WebApi REST (json) para facilitar el acceso a la red de aplicaciones clientes sin necesidad de adentrase en el protocolo.
* **mobile**: es un cliente desarrollado para Android 6.0 o superior que permite operar sobre la blockchain desde un tel&eacute;fono, tablet u otro dispositivo que cora Android. El c&oacute;digo fuente de este proyecto se encuentra en: https://github.com/nelopauselli/cryptovote-android

## Ejecuci&oacute;n de un nodo y el sitio web
El nodo puede correr tanto en Windows, Mac o Linux (incluso en Raspberry Pi as&iacute; como en docker).

1. El primer paso es descargar la versi&oacute;n del nodo y del sitio web correspondientes al sistema operativo:
* Windows x64: [TODO: poner link]
* Linux x64: [TODO: poner link]
* Linux arm (para Rasberry): [TODO: poner link]

2. descomprimir el archivo

3. ejecutar el nodo de **cryptovote**
```bash
# TODO: completar los [args]
# Windows:
> CryptoVote.exe [args] [args] [args]
# Linux:
$ TODO: escribir sintaxis
# OS X:
$ TODO: escribir sintaxis
```

4. ejecutar el sitio web de **cryptovote**
```bash
# TODO: completar los [args]
# Windows:
> Web.exe [args] [args] [args]
# Linux:
$ TODO: escribir sintaxis
# OS X:
$ TODO: escribir sintaxis
```

## Carga de datos de ejemplo
> TODO: crear y publicar un set de datos en archivos json para ser subidos utilizado **curl**

## Reglas de una elecci&oacute;n
> TODO: escribir reglas actuales de **cryptovote**

## Firmas de mensajes
cada mensaja est&aacute; firmado con claves asim&eacute;tricas como las utilizadas en bitcoin, as&iacute; verificamos que el mensajes realmente fue enviado por que dice ser el emisor.