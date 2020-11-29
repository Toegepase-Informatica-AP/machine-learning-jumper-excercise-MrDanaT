# Jumper with Intelligence Agent

<center>
Opdracht voor VR Experience

2020 - 2021

Auteurs:
<small>Dana Tabatabaie Irani</small>
<small>Alec Wuyts</small>
<small>Felix Neijzen</small>

</center>

## Inhoudstafel

1. [Intro](##-Intro)
2. [Spelverloop](##-Spelverloop)
3. [Observaties, acties en beloningen](##Observaties,-acties-en-beloningen)
4. [Setup deel 1: packages](##Setup-deel1:-packages)
5. [Setup deel 2: creatie van objecten](##Setup-deel-2:-creatie-van-objecten)
6. [Setup deel 3: het schrijven van scripts](##Setup-up-deel-3:-het-schrijven-van-scripts)
7. [Scènes](##-Scènes)
8. [Unity optimalisaties](##-Unity-optimalisaties)
9. [Trainingen](##Trainingen)

## Intro

In dit Unity project leren we een Intelligence Agent om over obstakels te springen door gebruik te maken van een beloningsysteem met munten. Deze ReadMe legt uit welke verschillende objecten en scripts actief zijn en hoe deze werken.

## Spelverloop

![Spelverloop scene-venster](https://i.imgur.com/t4SJ6dv.png)

De speler staat altijd op dezelfde positie op het veld t.o.v. de vloer en ziet verschillende objecten naar zich toe komen. Wanneer de speler een obstakel tegenkomt, kan hij er over springen. Wanneer de speler een munt tegenkomt, kan hij hier tegenaan botsen om extra punten bij te verdienen.

## Observaties, acties en beloningen

In dit project maken wij gebruik van reinforcement learning om de ML Agents op een correcte wijze te laten leren. Dit doen wij door gebruik te maken van zowel intrinsieke- als extrinsieke beloningen. Extrinsieke beloningen zijn beloningen die door ons worden gedefinieerd. Intrinsieke beloningen bepalen dan weer de nieuwsgierigheid van de ML Agents en hoe snel hij iets moet leren.

Zo zal _Player_ telkens gestraft of beloond worden per actie die hij onderneemt. In de volgende tabel staan deze extrinsieke beloningen en straffen opgesomd, gesorteerd volgens beloning.

| Omschrijving              | Beloning (floats) |
| ------------------------- | ----------------- |
| Aanraken van een obstakel | -1                |
| Springen                  | -0.15             |
| Stilstaan per frame       | +0.001            |
| Aanraken van een munt     | +0.6              |

> Een goede opmerking hierbij zou zijn dat het afstraffen voor het springen overbodig zou zijn als de agent al stilstaat en vice versa. Echter hebben wij ontdekt dat de combinatie van de twee voor veel snellere en betere resultaten kan zorgen. Zie paragraaf `Trainingen` voor meer informatie.

## Setup deel 1: packages

![ML Agents Package](https://i.imgur.com/o5zCZ1C.png)

Om aan machine learning te kunnen doen, zal de `ML Agents` package van Unity zelf geïnstalleerd moeten worden.

In dit project hebben wij geopteerd voor versie `1.0.6`.

## Setup deel 2: creatie van objecten

Vooraleer we aan de effectieve ML Agents training kunnen starten, zullen er eerst objecten aangemaakt moeten worden die als basis van dit project zullen dienen, beginnende bij het _environment_.

### _Environment_ object

![Environment](https://i.imgur.com/UVoXEGD.png)

Het _Environment_ is een parent object voor alle objecten in een enkele omgeving. Deze heeft ook het nodige gedrag om de spelomgeving foutloos te laten verlopen door continu _obstacles_ en _Coins_ te spawnen en te verzekeren dat er een _Player_ object bestaat.

Door meerdere _Environments_ in een scène te zetten, kan men meerdere spelomgevingen tegelijk laten draaien. Logischerwijs zal dit ervoor zorgen dat het leerproces van de ML Agents sneller zal verlopen.

> Opgelet: deze verhoogde leercurve is er enkel zolang het toestel waar je de training op draait krachtig genoeg zijn om al deze omgevingen tegelijkertijd te kunnen draaien. Als hier geen rekening mee gehouden wordt, zorgt dit juist voor een vertraging van het leerproces.

Bij het _Environment_ is het ook belangrijk om mee te geven welke prefabs het gebruikt om objecten te genereren, in dit geval zijn dit de de _Coin_, _Obstacle_ en _Player_ prefabs. Ook zal er een _Player_ object gegenereerd worden indien deze nog niet aanwezig is.

### _ScoreBoard_ object

![ScoreBoard](https://i.imgur.com/DRcItwm.png)

_ScoreBoard_ is een 3D-text display dat de huidige rewardscore laat zien. Deze heeft dan ook de tag "ScoreBoard".

### _Player_ object

![*Player prefab*](https://i.imgur.com/x7YRyic.png)

Het _Player_ object wordt voorgesteld door een simpele bean met een gezicht (twee ogen en een mond). Dit object staat stil op het veld en heeft enkel de "jump" actie. Deze actie kan _Player_ uit vrije wil nemen tijdens zijn trainingen of door de gebruiker tijdens het testen van het project. Omdat dit de enige beweging is die het object moet ondergaan, zijn de _X-_ en _Y-position_ waarden vastgezet op 0 in de Rigidbody, alsook de _X-_, _Y-_ en _Z-rotation_ waarden. Als laatste moet hier zeker het Decision Requester script op staan met "Take Actions Between Decisions" uitgevinkt.

Het object heeft een collider die gebruikt wordt in interacties met _Floor_, _Coin_ en _Obstacle_. Ook heeft het "_LeftEye_" en "_RightEye_" child sphere-objecten die de ogen voorstellen. Met Ray Perception Sensors, respectievelijk als Sensor Name LeftEyeDown en RightEyeDown (deze namen moeten uniek zijn), zorgen we ervoor dat het de _Coin_ en _Obstacle_ objecten kan zien. De ogen zijn elks 5° van het midden weggedraaid zodat ze minder overlappen.

Door het _Floor_ object aan te raken, weet de agent dat het opnieuw kan springen. De agent krijgt rewardscores wanneer _Player_ een _Coin_ aanraakt en verliest rewardscores als _Player_ een _Obstacle_ aanraakt.

Het is hier belangrijk dat de Behavior Name in Behavior Parameters "Player" is en overeenkomt met de behaviorname die zich in het configuratiebestand van ons Neural Network staat. Anders zal het trainen van de agent niet lukken.

De Ray Perception Sensors zijn als volgt ingesteld:
| Variabele             | Waarde         |
| --------------------- | -------------- |
| Detectable Tags       | Coin, Obstacle |
| Rays Per Direction    | 1              |
| Max Ray Degrees       | 5              |
| Sphere Cast Radius    | 3.5            |
| Ray Length            | 75             |
| Ray Layer Mask        | Mixed          |
| Stacked Raycasts      | 1              |
| Start Vertical Offset | 0              |
| End Vertical Offset   | -10            |
| Use Child Sensors     | True           |

Deze prefab heeft de tag "Player".

### _Floor_ object

![Floor prefab](https://i.imgur.com/ZMzZ1zZ.png)

Het _Floor_ object is een simpele vloer waar de andere objecten op "staan" en overheen bewegen. Verder heeft het geen gedrag om uit te kunnen voeren, maar wel een collider die wordt gebruikt voor de interacties met _Player_.

Dit object heeft de tag "Floor".

### _SpawnLine_ object

![SpawnLine prefab](https://i.imgur.com/eFaWhsp.png)

Het _SpawnLine_ object is een lijn die het einde van de vloer markeert en wordt gebruikt om de beginlocatie van objecten aan te duiden om ze daar te laten spawnen. Het heeft van zichzelf geen gedrag. Het enige waar men op moet letten is dat deze een trigger is. Zo voorkomen we dat onze gespawnde objecten hiertegen kunnen botsen.

Dit object heeft de tag "SpawnLine".

### _DeadLine_ object

![Deadline prefab](https://i.imgur.com/lBMlZlo.png)

Het _DeadLine_ object is een lijn die het einde van de vloer markeert en wordt gebruikt om objecten te destroyen. De Collider van dit object is een trigger. Als een _Coin_ of _Obstacle_ object voorbij dit lijn zou bewegen, wordt deze aldus gedestroyed. Dit zorgt ervoor dat er een soort garbage collection in ons spel is ingebouwd om de performantie te behouden.

Dit object heeft de tag "DeadLine".

### _Coins_ object

![Coins object](https://i.imgur.com/WyySf9g.png)

Een parent object voor de _Coin_ objecten die worden gespawned, gebruikt door het environment script.

### _Coin_ object

![Coin Prefab](https://i.imgur.com/f5CiQ5X.png)

Het _Coin_ object is een munt die bij _SpawnLine_ verschijnt en in een rechte lijn naar de _DeadLine_ (en de speler) beweegt. De collider wordt gebruikt in interacties met _Player_ en _DeadLine_. Deze collider is een trigger. Door gebruik te maken van een trigger, voorkomen we dat er een verplaatsing zal plaatsvinden bij de _Player_.

Als een _Coin_ het _Player_ object aanraakt krijgt de agent rewardscore.

Deze prefab heeft de tag "Coin".

### _Obstacles_ object

![Obstacles](https://i.imgur.com/ZogVTnt.png)

Een Parent object voor de _Obstacle_ objecten, gebruikt door het environment script.

### _Obstacle_ object

![Obstacle prefab](https://i.imgur.com/9TvamAR.png)

Het _Obstacle_ object is een balk die bij _SpawnLine_ verschijnt en in een rechte lijn richting de _DeadLine_ beweegt. De collider wordt gebruikt tijdens interacties met _Player_ en _Deadline_.

In tegenstelling tot het _Coin_ object, is dit geen trigger aangezien het botsen tegen het _Player_ object in het resetten van het spel zal resulteren.

Deze prefab heeft de tag "Obstacle".

## Setup up deel 3: het schrijven van scripts

Het gedrag van deze objecten werkt door gebruik te maken van scripts. Deze worden hieronder beschreven.

In onze scripts maken wij gebruik van de `transform.find` functionaliteit. Aangezien deze methode objecten zoekt op basis van hun tag, is het belangrijk dat alle objecten een correcte tag hebben.

In dit project worden de volgende tags gebruikt: Coin, DeadLine, Floor, Obstacle, Player, SpawnLine. Elk genoemd naar het overeenkomstige object te vinden in paragraaf `Setup deel 1`.

### _Environment_

Het environment script is verantwoordelijk voor het instantiëren van de verschillende objecten en voor het aanmaken van _Coin_ en _Obstacle_ objecten op een semi-willekeurige wijze.

Dit script bevat private objectinstanties van alle objecten in het project, zodat deze makkelijk toegankelijk zijn voor andere scripts, alsook de floats `maxTime`, `minTime`, `currentTimer` en `randomTimed`.

In de `Start()` functie begint `currentTimer` van waarde 0 en krijgt `randomTimed` een willekeurige waarde tussen `maxTime` en `minTime` door gebruik te maken van de `SetRandomTimed()` functie.

In de `OnEnable()` functie worden alle private variabelen zoals _coins_ geïnstantieerd.

Ook bevat dit script de functie `ClearEnvironment()` om _Coins_ en _Obstacles_ de verwijderen en het bord te resetten.

De functies `SpawnCoin()` en `SpawnObstacle()` worden gebruikt om `currentTimer` te resetten en vervolgens het corresponderend object te spawnen op de locatie van het _SpawnLine_ object met de correcte orientatie.

De functie `SpawnPlayer()` wordt gebruikt wanneer er geen _Player_ object meer aanwezig is in het environment. De locatie van deze spawn is relatief tot het environment en zal altijd correct geroteerd zijn zodat de Ray Perception Sensors richting de _SpawnLine_ gericht staan.

Het script bevat ook een `FixedUpdate()` functie. In deze functie wordt `currentTimer` incrementeel verhoogd en wordt er een `randomValue` tussen een kleine range(3) gegenereerd. Als `randomTimed` kleiner of gelijk is aan `currentTimer`, dan kan een object spawnen. Dit gebeurt afhankelijk van de `randomValue`; bij 1 spawnt een _Coin_, bij 2 een _Obstacle_ en bij 3 gebeurt er niets behalve een reset van `currentTimer`. Op deze manier is het gedrag van het environment zo goed als willekeurig waardoor de ML Agents geen patroon in het spawngedrag zouden kunnen herkennen.

### _Player_

Het _Player_ script bevat de float `jumpForce` die uitmaakt hoe hoog het object zal springen alsook de bool `jumpIsReady`. Ook heeft het script een Environment object om objecten uit het Environment te kunnen ophalen en zijn eigen Rigidbody object.

In de `Jump()` functie wordt het _Player_ object met snelheid `jumpForce` naar boven bewogen en zal daarna terug naar beneden vallen. Hierbij wordt `jumpIsReady` false en wordt er ook een kleine hoeveelheid rewardscore afgetrokken om willekeurig springen wanneer het niet nodig is af te straffen.

In de `FixedUpdate()` functie zal `RequestDecision()` aangeroepen worden als `jumpIsReady` true is. Deze zal op basis van de machine learning agent beslissen om te springen of niet.Als deze besluit om niet te springen krijgt de agent een zéér kleine hoeveelheid `rewardScore`. Hierdoor wordt hij aangemoedigd om op de grond blijven wanneer er niet gesprongen hoeft te worden.

Wanneer een nieuwe episode begint zal de functie `OnEpisodeBegin()` functie aangeroepen worden. Deze functie zal er voor zorgen dat er geen _Coin_ of _Obstacle_ pbjecten meer in het _Environment_ aanwezig zijn en dat er een _Player_ object bestaat indien dit nog niet het geval is.

Wanneer een object het _Player_ object aanraakt, wordt `OnCollisionEnter(Collider collision)` aangeroepen. Als dit een _Obstacle_ object is verliest de agent een grote hoeveelheid rewardscore en wordt het object vernietigd. Als het object het _Floor_ object is, wordt `jumpIsReady` opnieuw True.

Aangezien _Coins_ triggers zijn, wordt hiervoor de functie `OnTriggerEnter(Collider collision)` aangeroepen. Hierbij wordt de _Coin_ vernietigd en krijgt de agent een grote hoeveelheid rewardscore.

### _Coin_

Het _Coin_ script bevat de floats `givenSpeed`, `randomizedSpeed`, `maxSpeed` en `minSpeed`, alsook de boolean `constantGivenSpeed`. Gebaseerd op deze boolean, wordt het `randomizedSpeed` in de `Start()` functie oftewel gelijkgesteld aan de vaste waarde `givenSpeed`, oftewel aan een willekeurige waarde tussen de gegeven `maxSpeed` en `minSpeed`. Ook heeft het script een _Environment_ object om objecten uit het _Environment_ te kunnen ophalen.

_Coin_ heeft ook een `FixedUpdate()` functie, waarin de `Move()` functie wordt aangeroepen. Als `randomizedSpeed` hoger dan 0 is, dan wordt de `Move()` methode uitgevoerd en beweegt de _Coin_ in zijn forward richting aan snelheid `randomizedSpeed * Time.deltaTime`.

Ook heeft _Coin_ een `OnCollisionEnter(Collider other)` functie, deze evalueert of de Collider die het aanraakt die van een _DeadLine_ is. Als dit het geval is, dan wordt de _Coin_ verwijderd.

### _Obstacle_

Het _Obstacle_ script is synoniem aan het _Coin_ script.

## Scènes

Voor een duidelijke afbakening te maken tussen de testomgeving van het verkregen brein van de Intelligence Agents en de leeromgeving van deze agents, zijn er twee scènes gemaakt.

### _SampleScene_

![*SampleScene*](https://i.imgur.com/MVnF3wY.png)

In de _SampleScene_ scène zal er slechts één _Environment_ getoond worden waar het verkregen brein van de Intelligence Agents op de (enige) _Player_ gezet zal worden. Zo wordt de aandacht op één specifiek _Environment_ gelegd. Op deze mannier kan er een makkelijke switch gemaakt worden tussen het leren en het testen. Ook zorgt dit natuurlijk voor veel minder resource (zowel CPU-, als GPU-) verbruik.

### _ML Scene_

![*ML Scene*](https://i.imgur.com/GK5SZg5.png)

Op de _ML Scene_ worden er tien _Environments_ geplaatst. Wij hebben voor dit aantal geopteerd aangezien dit voor een perfecte harmonie zorgt tussen het CPU- en GPU-verbruik.

In eerste instantie zou het niet lijken dat zo'n eenvoudig _Environment_ veel resources zou gebruiken. Echter moet er rekening worden gehouden met het feit dat er enorm veel tegelijk gebeurt. Voorbeelden hiervan zijn: per **x** aantal seconden worden er nieuwe objecten aangemaakt en verwijderd, elke _Player_ heeft zijn eigen _RayPerceptionSensoren_ wat zeer CPU-intensief is, etc.

Hierdoor worden er nog enkele optimalisaties uitgevoerd op onze Unity omgeving om dit vlekkeloos te laten gebeuren.

## Unity optimalisaties

Aangezien dit project zich focust op ML Agents die het spel leren spelen, is er geen nood aan enkele configuraties van ons Unity project en "mooie" textures die onze performantie op negatieve wijze kunnen beïnvloeden. Hiervoor doen wij het volgende:

- In de Game-window staat de **aspect ratio** fixed op `4:3` met `Low Resolution Aspect Ratios` ingeschakeld.
- De **lighting settings** aangepast zodat deze niet gebruikt maakt van de `Skybox`, maar wel van een `fixed kleur`, nl. `wit` met **Ambient Mode** op `Baked`.
- Het effectief `baken` van uw lightings zodat deze niet @Runtime worden gecalculeerd.
- Tijdens het **runnen** van de ML Agents de Game-window en Scene-window effectief **afsluiten** zodat er geen onnodig CPU- en GPU-verbruik is.

![Lighting Settings](https://i.imgur.com/GZP0N7y.png)

## Trainingen

In de parent folder van ons Unity project maken we een folder aan genaamd `Learning` waar alle resultaten van het Neural Network in zullen komen.

Hierin maken we als eerste een `Player-01.yml` configuratie-bestand. Hierin komen de instellingen van ons Neural Network in terrecht. Wij hebben geopteerd voor de volgende configuratie:

### Configuratiebestand

```yml
behaviors:
  Player:
    trainer_type: ppo
    max_steps: 5.0e7
    time_horizon: 64
    summary_freq: 10000
    keep_checkpoints: 5
    checkpoint_interval: 50000

    hyperparameters:
      batch_size: 32
      buffer_size: 9600
      learning_rate: 3.0e-4
      learning_rate_schedule: constant
      beta: 5.0e-3
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3

    network_settings:
      num_layers: 2
      hidden_units: 128
      normalize: false
      vis_encoder_type: simple

    reward_signals:
      extrinsic:
        strength: 1.0
        gamma: 0.99
      curiosity:
        strength: 0.02
        gamma: 0.99
        encoding_size: 256
        learning_rate: 1e-3
```

De Intelligence Agents kunnen getraind worden d.m.v. mlagents van Python. Hiervoor wordt [versie 3.8.6](https://www.python.org/downloads/release/python-386/) aangeraden.

Na het installeren van Python, kan men door het ingeven van `pip install mlagents` de installatie voltooien om te kunnen starten met trainen.

Een training kunnen we starten door `mlagents-learn Player-01.yml --run-id Player-01` in de command prompt in te geven en nadien ons Unity project te laten draaien.

### Samenvatting trainingen

![Samenvatting trainingen](https://i.imgur.com/YlE2b3a.png)

Na +50u aan traingen te doen en allerlei mogelijke configuratie bestanden en vormen van rewards (meermaals) te testen, komen we op het uiteindelijke resultaat dat, voor het beste en meest natuurlijk gedrag van de agent, dit bovenstaand configuratiebestand, in combinatie met het afstraffen van punten voor het springen en belonen om stil te staan, de beste combinatie is.

![Top 3 combinaties](https://i.imgur.com/W44ACvU.png)

![Overzicht van alle grafieken](https://i.imgur.com/oOUZfkR.png)
