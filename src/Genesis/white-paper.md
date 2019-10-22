---
Version: 1.0
Author: Nelo Pauselli
---
# Crypto Vote White Paper
Este es un proyecto para desarrollar un software de votaciones con tecnologías Blockchain.

## Objetivo
Tener un software capaz de gestionar una base de datos descentralizada donde se escriban cada voto de distintas elecciones y cada individuo pueda ser a la vez fiscal de esas votaciones.

## Primeras pregunta
* ¿por qué no deberíamos armar esto como un contrato inteligente?
* ¿por qué no deberíamos subirnos a una Blockchain ya existente?
* ¿por qué no escribir los resultados en una Blockchain ya robusta? ¿podría ser esto una característica opcional?

## Principios
1. Toda la información que sea necesaria para interpretar las votaciones debe estar escrita en la blockchain, incluyendo el presente documento ¿si?
2. Cualquiera puede descargarse el código fuente y empezar su propia Blockchain de votaciones.
3. La forma de obtener los tokens es minando la Blockchain.
4. La forma de pagar por escribir un registro es con tokens.
5. Los tokens no pueden ser transferidos ¿mmm?
> El punto 5 sería para evitar la comercialización de los tokens... no estamos acá por la plata sino por una Blockchain pública como registro de votaciones.... ¿querés tokens? => miná!
6. El minado debe ser factible de realizarse en un PC / Notebook / Raspberry / ¿Esp? / ¿¿Arduino??.

## Contenido de la Blockchain
En esta Blockchain se escribirán 5 tipos de registros:
* **BigBag** será el contenido del bloque *génesis* y dará inicio a la cadena, probablemente contenga este white paper y genere los Tokens ¿iniciales? con que se fomentará el uso de la red. ¿se incluirá en el bloque *génesis* la primera organizaciones?
* **Organizaciones** agrupan electores así como temas a votar, cada elector puede votar los temas de las organizaciones a las que pertenece. ¿Tanto la validez de los electores como de los temas son responsabilidad de la organización, la cual firmará con su clave asimétrica cada creación de un tema o asociación de un elector?
* **Temas** son los asuntos sujetos a votación, incluye las opciones a votar.
* **Electores** son las direcciones (claves públicas) habilitadas a votar dentro de una organización.
* **Votos** represanta la opción elegida por cada Elector en cada Tema que desea votar de las Organizaciones a las que pertenece.

## Tokens (BALLOT)
### Uso
Los tokens se usan para *pagar* el minado de cada votación que alguien quiera ejecutar en la red.
### Obtención
La forma de obtener estos tokens es minando la Blockchain.

## Caso de Uso
1. La EANT se baja el minero e inicia una nueva cadena de bloques, así entonces se constituye como **Organización** inicial poniendo su *clave pública*, firmando el registro y minando el bloque génesis. Así obtiene la totalidad de BALLOTs de la cadena.
2. En un futuro bloque de la cadena, La EANT certifica la *clave pública* del Colegio de Sociólogos de la Provincia de Buenos Aires, firmando con sus *claves asimétricas* el registro(cuya *clave pública* está certificada por la EANT) e indicando la cantidad de BALLOTs que paga por minar dicha certificación. ¿o debería tener un valor en BALLOTs predefinido?
3. El Colegio de Sociólogos de la Provincia de Buenos Aires agrega electores a su padrón ¿por lote?, lo cual se escribe en la cadena de bloques y tiene un costo en BALLOTs ¿predefinido?.
4. El Colegio de Sociólogos de la Provincia de Buenos Aires exponer un tema a votación, firmando el registro con sus *claves asimétricas*, indicando cuantos BALLOTs *paga* para llevar esta votación adelante.
5. Los Mineros deben validar que los votos de un tema vengan de votantes habilitados en esa Organización, que hayan sido emitidos en el período en que estuvo activa la elección y que hayan votado una sola vez ¿o se toma el último voto de cada elector? ¿o el primero?
6. Al finalizar la elección ¿se escribe en la cadena de bloques el resultado o será mejor que siempre se deba volver a contar el resultado?

# Código fuente
Acá está todo el código fuente: [https://github.com/nelopauselli/cryptovote](https://github.com/nelopauselli/cryptovote)

