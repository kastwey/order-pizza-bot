# Taller de bots con Bot Framework

## 1. Introducción

En este taller, veremos cómo construir un bot paso a paso, empezando desde el bot más simple (un bot que repite todo lo que decimos, hasta un pequeño bot para pedir pizzas a un restaurante fictício.

El objetivo de este taller es que  podáis aprender los fundamentos de un  bot, y de manera práctica, entendáis cómo Bot Framework nos provee de todo lo necesario para crearlo.

## 2. Requisitos

- La última versión de uno de estos dos productos:
 * [Microsoft Visual Studio 2019 community edition para Windows](https://visualstudio.microsoft.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=16#)
 * [Visual Studio Community Edition para Mac](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio-mac/?sku=communitymac&rel=16).
- La última versión de la herramienta Bot Framework Emulator V4:
 * [Bot Framework Emulator para Windows](https://github.com/microsoft/BotFramework-Emulator/releases/download/v4.7.0/BotFramework-Emulator-4.7.0-windows-setup.exe)
 * [Bot Framework Emulator para Mac](https://github.com/microsoft/BotFramework-Emulator/releases/download/v4.7.0/BotFramework-Emulator-4.7.0-mac.dmg)
 * [Bot framework emulator para Linux](https://github.com/microsoft/BotFramework-Emulator/releases/download/v4.7.0/BotFramework-Emulator-4.7.0-linux-x86_64.AppImage)
- Una cuenta de estudiante de Azure (la dirección de correo de la UB os debería valer para el registro): [Register in azure for students](https://azure.microsoft.com/es-es/free/students/).
- Git:
 * [Git for Windows](https://github.com/git-for-windows/git/releases/download/v2.25.1.windows.1/Git-2.25.1-64-bit.exe)
 * [Git for Mac](https://sourceforge.net/projects/git-osx-installer/files/git-2.23.0-intel-universal-mavericks.dmg/download?use_mirror=autoselect)
* Node (la última versión disponible): 
 * [Node.JS para Windows](https://nodejs.org/dist/v12.16.1/node-v12.16.1-x64.msi)
 * [Node.JS para MAC](https://nodejs.org/dist/v12.16.1/node-v12.16.1.pkg)

### 2.1. Actualizando Visual Studio

Si ya teníais Visual Studio 2019 instalado, comprobad que 
tenéis la última versión .
Para ello, bajaos el instalador de la sección anterior y ejecutadlo. Si os pide actualizar, hacedlo, ya que necesitamos la última versión que contenga el SDK de .Net Core 3.1.

Si ya tenéis la versión 16.4 o superior de Visual Studio y no queréis actualizar, comprobad que tenéis instalado el SDK de .Net Core 3.1:

Desde la consola cmd:
```bash
C:\>dotnet --info
[...]
.NET Core SDKs installed:
  2.1.701 [C:\Program Files\dotnet\sdk]
  2.2.301 [C:\Program Files\dotnet\sdk]
  3.1.101 [C:\Program Files\dotnet\sdk]
```

Si no os aparece una versión 3.1.x, [podéis descargaros el SDK de .Net Core 3.1 desde aquí](https://dotnet.microsoft.com/download/dotnet-core/3.1) e instalarlo, ya que es compatible desde la versión 16.4 de Visual Studio. Recordad elegir la versión correcta para vuestro sistema operativo.

### 2.2. Configurando Visual Studio

Cuando ejecutéis el instalador por primera vez, os saldrá la pantalla de configuración. En la pestaña carga de trabajo (o workload, si os aparece en inglés), marcad al menos lo siguiente (yo os recomiendo que si queréis cacharrear con el entorno y sus posibilidades marquéis más cosas ;) ):
* Desarrollo de Asp.Net y Web.
* Desarrollo multiplataforma de .Net Core

En la pestaña de idiomas, aseguraos de que el idioma Inglés está marcado, ya que es el idioma que utilizaremos para las instrucciones.
Una vez instalado, abrid Visual Studio. Si la interfaz os aparece en español, id a herramientas > Opciones, marcad la pestaña "Configuración internacional" y elegid el idioma Inglés. Si este idioma no os aparece ,aseguraos que está marcado en el instalador. Podéis volver a ejecutarlo, pulsad en Modificar y marcarlo.

En windows, para acabar de configurarlo, vamos a elegir el esquema de teclas rápidas que tendrá nuestro Visual Studio. Para ello, nos iremos a Tools > Options > Keyboard. En esta pestaña, elegiremos como esquema Visual C# 2005 y pulsaremos en "OK"..
Como yo no puedo usar ratón, soy el rey de las teclas rápidas, así que lo que os explique en el taller será siempre utilizando teclas en lugar de ratón. Si esto lo tomáis por costumbre cuando aprendáis un entorno nuevo, os manejaréis por él con una rapidez inusitada ;).

### 2.3. Descargando el repositorio

Desde la consola cmd (o terminal de Mac), ejecutad:

```bash
C:\> git clone https://github.com/kastwey/order-pizza-bot
```

Esto os descargará la última versión del repositorio. Aseguraos de que estamos en la rama master:

```
c:\>git checkout master
Already on 'master'                                                                                                     
Your branch is up to date with 'origin/master'.                                                                        
```

## 3. Estructura del repositorio

Dentro del repositorio, podréis encontrar la carpeta src, con todo el código fuente. Dentro de esta carpeta, veréis distintas subcarpetas:
* 00-TheMuteBot
* 01-BasicTemplate
* 02-LuisIntents
* 03-FirstWaterfallDialog
* 04-AddingInterruptions
* 05-ImprovingConfirmationDialogs
* 06-PredefinedPizzas
* 07-CustomPizzas
* 08-OrderStatus
* FullSolution

Cada una de estas carpetas contiene un paso en la construcción de nuestro bot: desde la primera en la que tenemos un simple bot que repite lo que decimos, hasta el paso 8, en el que tenemos un bot que nos permitirá pedir pizzas predefinidas y personalizadas.

En la carpeta "FullSolution" está todo el código, incluyendo la característica de desbloquear un paso de un diálogo cuando se han producido demasiados reintentos.
## 4. Empezando a trabajar

En este taller, recorreremos todos los pasos mencionados anteriormente, en los que observaremos cómo se van añadiendo funcionalidades a nuestro BOT.

### 4.1. Primer paso: 00-TheMuteBot

Este es el bot mudo. NO hace nada (por ahora)

* Id al explorador de soluciones (ctrl + w, s) y 
navegad hasta OrderPizzaBot > Bots > OrderPizzaBot.cs
* Bajad hasta el método
 ```csharp
protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
 ```
 Este evento se ejecuta cuando llega una actividad de mensaje. Ahora mismo no contiene nada, por lo que basicamente espera a que el usuario le escriba otra cosa (es un bot mudo).
 Incluid el siguiente código en el método:
 ```csharp
var replyText = $"¡Eco eco! {turnContext.Activity.Text}";
			await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
 ```
 Compilad, (F6), y levantad el bot (F5). Os aparecerá una página que os dirá que os podéis conectar al bot. Copiad esa dirección.
* Abrid el "Bot Framework Emulator", pulsad en File > Open bot, y pegad la dirección que habéis copiado en el paso anterior.
* Pulsad en conectar, y veréis que tenéis un cuadro para escribir.
 Cuando le escribáis algo al BOT, este os contestará con Eco eco, y lo que le digáis. Podéis parar el proyecto (shift + f%), cambiar algo en la cadena de respuesta, compilar, volver a ejecutar, y ver cómo el bot contesta ahora de forma diferente.

### 4.2. Trabajando con Luis.

Luis es un sistema de inteligencia artificial que intenta comprender el lenguaje natural.
En este paso, vamos a añadir Luis a nuestro bot.
* Id al portal de azure: [https://portal.azure.com](https://portal.azure.com).
* Iniciad sesión con la cuenta que creásteis en los requisitos.
* Si el portal os sale en español, id a configuración, pestaña "Idioma y región", y ahí elegid Inglés. Aplicad los cambios
* En el portal, haced click sobre el cuadro de búsqueda, escribid: luis, y elegir language understanding
* En la pantalla de creación, elegid el botón de radio "authoring". Marcad la suscripción en la que se va a crear el recurso (la única que os aparecerá).
* En grupo de recursos, Cread un nuevo grupo de recursos. escribid order-pizza-bot
* Como nombre de la aplicación, poned un nombre único: ub-order-pizza-bot-tunombre
* Como authoring location: us: west US.
* En Authoring pricing tier, elegid la capa gratuita F0.
* Pulsad en Review & create, y a continuación, en Create.
* Tardará un poco, esperad a que el portal de Azure os notifique que ya se ha creado el recurso de creación (authoring resource).
* Cuando os aparezca la pantalla indicándoos que se ha creado, pulsad en "Go To Resource".
* En la pantalla que aparece, copiad la clave del recurso (key 1) y guardadla a buen recaudo. Copiad también el endpoint.
* Id al portal de luis: [https://preview.luis.ai](https://preview.luis.ai). Este es la versión preliminar del portal, pero nos servirá mejor para lo que vamos a hcer, ya que es más completa.
* Pulsad en "Sign In". Os debería aparecer directamente la página de Microsoft, pidiéndoos que aceptéis a la aplicación de Luis como aplicación autorizada.
* Pulsad en Aceptar.
* Aceptad los términos de uso y pulsad en Aceptar.
* En el menú del portal, pulsad "My apps" y, a continuación, pulsar en "New app for conversation".
* Rellenar los campos:
 * Name: ub-order-pizza-bot-vuestronombre. Yo puse: ub-order-pizza-bot-jmontiel.
 * Culture: spanish.
 * Description: Un bot para pedir pizza.
* Pulsad en Done.
* Nos aparecerá la pantalla de Intents. Los intents son las intenciones de nuestra aplicación, aquello que queremos que nuestro bot pueda entender. Vamos a crear varios, que utilizaremos más adelante en nuestro bot.

#### 4.2.1. Intent Greet

Lo primer oque haremos será pulsar en el botón "Create" para crear nuestro intent.
* Intent name: Greet.
* Pulsad en Done.

Ahora, se nos abrirá la pantalla de utterances (declaraciones). Aquí se trata de que nuestro sistema de inteligencia artificial entienda qué frases van a desencadenar esta intención. Mientras más frases pongamos, mejor podrá detectar nuestra intención. Unas quince frases estarían bien para empezar.
Para cada intent, tendremos que poner más o menos el mismo número de frases, y tenemos que intentar que las frases de distintos intents no sean muy similares, pues el bot puede confundirse.

* hola
* hola caracola
* Buenas
* buenas tío
* buenas, tío
* qué pasa
* que pasa
* buenos días
* buenos dias
* buenas tardes
* buenas noches
* qué tal
* que tal
* Cómo estás
* como estas
* cómo va eso
* como va eso
* saludos, terrícola
* saludos

Como ves, es importante usar también faltas de ortografía, distinta gramática y distintos signos de puntuación. ¡No todos escriben igual!

#### 4.2.2. OrderPizza

Vamos a añadir un nuevo intent. Pulsad en el menú el ítem "Intents" y repetid el proceso del punto anterior. Ahora, nuestro intent se llamará OrderPizza.

Las declaraciones podrían ser algo como:

* quiero pizza
* quiero una pizza
* quiero dos pizzas
* Quiero pedir una pizza
* quiero pedir siete pizzas
* quiero pizza a domicilio
* quiero una pizza a domicilio
* quiero ocho pizzas a domicilio
* quiero pedir una pizza a domicilio
* quiero pedir siete pizzas a domicilio
* quiero una pizza para casa
* Tráeme una pizza
* Tráeme dos pizzas
* Traeme una pizza
* traeme dos pizzas
* quiero pizza a recoger
* quiero una pizza a recoger
* Quiero ocho pizzas a recoger
* quiero pizza para recoger
* quiero una pizza para recoger
* quiero cuatro pizzas para recoger
* pedir una pizza
* pedir pizza
* pedir siete pizzas
* pedir una pizza a domicilio
* pedir tres pizzas a recoger

De estos intents sacaremos entidades. Para ello, vamos a crearlas.

* pulsad en el ítem "Entities" y, a continuación, en el botón "Add prebuilt entity"
* Vamos a crear una entidad de tipo numérico. Marcamos la fila y pulsamos en Done.
* A continuación, pulsaremos en "Create" (vamos a crear una nueva entidad).
 * Name: OrderType.
 * Type: List.
* Pulsad en Next.
* En sublists, cread dos ítems:
 * Pickup
 * Delivery
* En la tabla de ítems de lista, para los sinónimos, escribid:
 * En PickUp: recoger, voy, recojo
 * En delivery: domicilio, tráeme, traeme, tráemela, traemela, trae, casa
* Pulsamos en Create.
* Ahora, si volvemos a Intents, y elegimos "OrderPizza", veremos que todas las declaraciones que tienen entidades las está cogiendo correctamente.

#### 4.2.3. OrderStatus

Este intent nos servirá para pedirle al sistema que nos lea el estado de nuestro pedido.

* Nombre: OrderStatus.
* Declaraciones:
 * cómo va mi pedido
 * como va mi pedido
 * dime como va mi pedido
 * le falta mucho a mi pedido
 * estado de mi pedido
 * estado del pedido
 * tengo hambre
 * cómo lleváis mi pedido?
 * ... (lo que se os ocurra) ;)

#### 4.2.4. Reject

Este intent servirá para buscar negaciones, cuando rechazamos una acción.

Como declaraciones, escribid todo aquello que consideréis que puede ser una negación: no, ni de broma, qué va...

#### 4.2.5. Accept

Al contrario que Reject, esta intención servirá para aceptar una acción. POdríamos escribir como declaraciones: sí, si, claro, por supuesto, de acuerdo, ETC.


#### 4.2.6. GetMenu

Este intent servirá para que los usuarios pidan la carta a la pizzería.

* Quiero el menú
* quiero el menu
* menú
* menu
* dame el menú
* dame el menu
* dame la carta
* léeme la carta
* leeme la carta
* carta
* la carta
* ...

#### 4.2.7 Testeando nuestra aplicación.

Para testear nuestras intenciones, lo primero es entrenar nuestra aplicación con las intenciones que hemos añadido.

Pulsad en el botón "Train". Os aparecerá una pantalla de progreso. Cuando finalice, pulsad en el botón "Test".

Esta pantalla nos permite testear frases. Luis nos dirá qué cree que es, y así, podremos realizar las correcciones oportunas.

Jugad con el cuadro de test, y si la intención que queréis da una intención incorrecta, editad la intención y añadid vuestra frase y frases similares para corregirlo. Tras añadir cualquier frase, tendréis que guardar y volver a entrenar.

#### 4.2.8. Publicando nuestra aplicación

Ha llegado el momento de publicar nuestra aplicación.

* Pulsad el botón "Publish"
* En la pantalla que os aparece, marcad la franja de producción (production slot).
* Pulsad en "Done". Cuando se haya publicado, os avisará mediante una notificación.
* Ahora, pulsad en Manage > Application settings.
* En "App ID", copiad el valor y guardadlo a buen recaudo junto con la clave que copiásteis anteriormente en el "authoring resource" de Azure.
* Ahora, id a manage > Versions.
* Seleccionad la versión publicada, y pulsad en export > json file.
* Descargad el fichero json, que contendrá toda vuestra aplicación: intenciones, declaraciones, entidades, ETC y guardadlo, que luego lo usaremos.

### 4.3. Incluyendo luis en nuestro bot

Ha llegado el momento de incluir luis en nuestro bot. Para ello, utilizaremos la plantilla de nuestro bot mudo y la iremos mejorando en los siguientes pasos.

Los ficheros con el código los tenéis en la carpeta workshop-files, con una subcarpeta por cada caso.

* Cread una nueva carpeta dentro del proyecto llamada "Dialogs".
* Dentro de la carpeta, cread una nueva clase llamada "MainDialog".
* Copiad el contenido del fichero workshop-files\02-LuisIntents\MainDialog.txt en esa clase.
* Añadid una segunda clase en la raíz del proyecto (no de la solución) llamada OrderPizzaRecognizer, y pegad el contenido dle archivo OrderPizzaRecognizer.txt.
* Abrid el fichero startup.cs, y sustituidlo por el contenido del archivo startup.txt
* Abrid el fichero bots > OrderPizzaBot.cs y sustituid el contenido de ese fichero por el del fichero OrderPizzaBot.txt
* Ahora, tendremos que instalar la herramienta luisgen (por lo que en los requisitos os pedí que instaláseis node.js). En la consola, ejecutad:

```bash
npm install luisgen -g
Tool 'luisgen' (version '2.2.0') was successfully installed.                                                            
+ luisgen@2.2.0                                                                                                         
added 1 package in 4.62s                                                                                                
```
* Cread una carpeta en el proyecto llamada CognitiveModels.
 * Copiad el fichero 0.1.json descargado desde la versión de la aplicación a esa carpeta, con el nombre OrderPizza.json
* En la consola, situaos en el directorio donde está este fichero, por ejemplo:
 ```bash
C:\>cd proyectos\order-pizza-bot\src\02-LuisIntents\OrderPizzaBot\CognitiveModels
 ```
* Ahora, ejecutamos, dentro de esa carpeta:
 ```bash
C:\proyectos\order-pizza-bot\src\02-LuisIntents\OrderPizzaBot\CognitiveModels>luisgen OrderPizza.json -cs OrderPizzaBot.OrderPizza                                                                                                              
Generating file OrderPizza.cs that contains class OrderPizzaBot.OrderPizza.
 ```

¡Ya tendremos todo lo necesario para ejecutar nuestra aplicación! Pulsad f6 para compilar, y f5 para levantar el bot.
Conectaos con Bot Framework y ejecutad las intenciones que queráis. El bot os responderá con la intención que cree haber interpretado, y la confianza en esa decisión.

### 4.4. Nuestro primer diálogo en cascada

Ahora, vamos a crear nuestro primer diálogo en cascada.

Este tipo de diálogo consta de varios pasos, que se van sucediendo de manera secuencial. En esta evolución del bot, vamos a crear un diálogo de pasos para gestionar el menú principal, y otro para gestionar el número de pizzas y el tamaño.

Partiendo del bot mudo que hemos modificado para añadir luis, sigamos adelante:

* abre el fichero Bots > OrderPizzaBot.cs y sustituye su contenido por el fichero que se encuentra en los archivos del workshop (workshop-files), en la carpeta "03-FirstWaterfallDialog", con el nombre OrderPizzaBot.txt
* Dentro de la carpeta Dialogs, crea una nueva clase llamada OrderPizzaDialog.
* Abre el fichero OrderPizzaDialog.cs recién creado, y sustituye su contenido por el del archivo OrderPizzaDialog.txt.
* Ahora, abre el fichero MainDialog.cs, y sustituye su contenido por el del archivo del workshop MainDialog.txt
* Crea una nueva carpeta llamada Entities, y crea tres clases: Enums y OrderInfo, sustituyendo su contenido por su contrapartida txt dentro de los ficheros del workshop.
* Crea una carpeta llamada Extensions, y dentro, una clase llamada EnumExtensions, sustituyendo su contenido por el fichero EnumExtensions.txt.
* Abre el fichero Startup.cs, y sustituye el contenido por el del fichero Startup.txt.

Compila con f6 y ejecuta con F5.

Ahora, veréis que, si le pedimos una pizza con números, lo detectará y nos pedirá confirmación. Si le falta alguna entidad, nos la pedirá antes de confirmar.
También podremos preguntarle por alguna de las otras opciones que le pedimos en el intent.
