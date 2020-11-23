# Jumper with Intelligence Agent

## Intro

In dit Unity project leert een AI om over obstakels te springen door gebruik te maken van een beloning systeem met munten, deze ReadMe legt uit welke verschillende objecten en scripts actief zijn en hoe zij werken.

## Spelverloop

![Spelverloop](https://i.imgur.com/t4SJ6dv.png)

De speler staat op het veld en ziet verschillende objecten naar zich toe komen. Wanneer de speler een obstakel tegenkomt, kan hij er over springen om punten te verdienen. Wanneer speler een munt tegenkomt, kan hij hier tegenaan botsen om extra punten bij te verdienen.

## Observaties, acties en beloningen

In dit project maken wij gebruik van reinforcement learning om de ML Agents op een correcte wijze te laten leren. Dit doen wij door gebruik te maken van zowel intrinsieke- als extrinsieke beloningen. Extrinsieke beloningen zijn beloningen die wijzelf definiëren. Intrinsieke beloningen bepalen dan weer de nieuwsgierigheid van de ML Agents en hoe snel hij iets moet leren.

Zo zal *Player* telkens gestraft of beloond worden per actie die hij onderneemt. In de volgende tabel staan deze extrinsieke beloningen en straffen opgesomd, gesorteerd volgens beloning.

| Omschrijving          | Beloning (floats) |
| --------------------- | ----------------- |
| Aanraken van een muur | -1                |
| Springen              | -0.15             |
| Stilstaan per frame   | +0.001            |
| Aanraken van een munt | +0.6              |

## Objecten

Vooraleer we aan de effectieve ML Agents training kunnen starten, zullen er eerst objecten aangemaakt moeten worden die als basis van dit project zullen dienen, beginnende bij *environment*.

### *Environment*

![Environment](https://i.imgur.com/rfNqeFy.png)

Het *environment* is een parent object voor alle objecten in een omgeving. Deze voert ook gedrag uit om de spelomgeving te laten verlopen, zoals het spawnen van objecten en *Coins* en verzekeren dat er een *Player* object bestaat. Door meerdere *environments* in een scène te zetten, kan men meerdere spelomgevingen tegelijk laten draaien. Logischerwijs zal dit ervoor zorgen dat de leercurve van de ML Agents sneller zal verlopen.

### *ScoreBoard*

![ScoreBoard](https://i.imgur.com/BLF7NF9.png)

*ScoreBoard* is een text display dat de huidige rewardscore laat zien.  

### *Player*

![*Player prefab*](https://i.imgur.com/OJApfgI.png)

Het *Player* object wordt voorgesteld door een simpele bean met een gezichtje (twee ogen en een mond). Dit object staat stil op het veld en heeft enkel de "jump" actie. Deze actie kan *Player* uit vrije wil nemen gebaseerd op machine learning.

Het heeft een collider die gebruikt wordt in interacties met *Floor*, *Munt*, *Coin* en *Obstacle*. Ook heeft het een "*LeftEye*" en "*RightEye*" child sphere-object die de ogen voorstellen en met Ray Perception Sensors ervoor zorgen dat het de *Coin* en *Obstacle* objecten kan zien.

Door het *Floor* object aan te raken weet de agent dat het opnieuw kan springen. De agent krijgt rewardscores wanneer *Player* een *Coin* aanraakt en verliest rewardscores als *Player* een *Obstacle* aanraakt

### *Floor*

![Floor prefab](https://i.imgur.com/ZMzZ1zZ.png)

Het *Floor* object is een simpele vloer waar de andere objecten op "staan" en overheen gaan, het heeft zelf geen gedrag om uit te voeren. Het heeft een collider die gebruikt word voor interacties met *Player*

### *SpawnLine*

![SpawnLine prefab](https://i.imgur.com/eFaWhsp.png)

Het *SpawnLine* object is een lijn die het einde van de vloer markeert en gebruikt word om objecten te spawnen, het heeft van zichzelf geen gedrag.

### *DeadLine*

![Deadline prefab](https://i.imgur.com/lBMlZlo.png)

Het *DeadLine* object is een lijn die het einde van de vloer markeert en gebruikt word om objecten te destroyen en daarbij de rewardscore aan te passen.

### *Coins*

![Coins object](https://i.imgur.com/WyySf9g.png)

Een parent object voor de *Coin* objecten, gebruikt door het environment script.

### *Coin*

![Coin Prefab](https://i.imgur.com/f5CiQ5X.png)

Het *Coin* object is een munt die bij *SpawnLine* verschijnt en in een rechte lijn naar de *DeadLine* (en de speler) beweegt. De collider wordt gebruikt in interacties met *Player* en *DeadLine*.

Als een *Coin* het *Player* object aanraakt krijgt de agent rewardscore.

### *Obstacles*

![Obstacles](https://i.imgur.com/ZogVTnt.png)

Een Parent object voor het *Obstacle* object, gebruikt door het environment script.

### *Obstacle*

![Obstacle prefab](https://i.imgur.com/9TvamAR.png)

Het *Obstacle* object is een balk die bij *SpawnLine* verschijnt en in een rechte lijn naar de *DeadLine* beweegt. De collider wordt gebruikt in interacties met *Player* en *Deadline*.

Als een *Obstacle* het *Player* object aanraakt verliest de agent rewardscore.

## Scripts

Het gedrag van deze objecten werkt door gebruik van scripts, deze staan hieronder beschreven.

### Environment

Het environment script is verantwoordelijk voor het instantiëren van de verschillende objecten en voor het aanmaken van *Coin* en *Obstacle* objecten op een semi-willekeurige wijze.

Dit script bevat private objectinstanties van alle objecten in het project, zodat deze makkelijk toegankelijk zijn voor andere scripts, alsook de floats `maxTime`, `minTime`, `currentTimer` en `randomTimed`.

In de `Start()` functie begint `currentTimer` van waarde 0 en krijgt `randomTimed` een waarde tussen `maxTime` en `minTime`.

In de `OnEnable()` functie worden alle private variabelen zoals *coins* geïnstantieerd.

Ook bevat dit script de functies `ClearEnvironment()` om *Coins* en *Obstacles* de verwijderen en het bord te resetten.

De functies `SpawnCoin()` en `SpawnObstacle()` worden gebruikt om `currentTimer` te resetten en vervolgens het corresponderend object te spawnen op de locatie van het *SpawnLine* object met de correcte orientatie.

Het script bevat ook een `FixedUpdate()` functie, in deze functie wordt `currentTimer` incrementeel verhoogd en wordt er een `randomValue` tussen een kleine range(3) gegenereerd. Als `randomTimed` kleiner of gelijk is aan  `currentTimer`, dan kan een object spawnen. Dit gebeurt afhankelijk van de `randomValue`; bij 1 spawnt een *Coin*, bij 2 een *Obstacle* en bij 3 gebeurt er niets behalve een reset van `currentTimer`. Op deze manier is het gedrag van het environment semi-willekeurig waardoor de ML Agents geen patroon in het spawngedrag zouden kunnen herkennen.

### *Player*

Het *Player* script bevat de float `jumpForce` die uitmaakt hoe hoog het object zal springen alsook de bool `jumpIsReady`. Ook heeft het script een Environment object om objecten uit het Environment te kunnen ophalen en zijn eigen Rigidbody object.

In de `Jump()` functie word het speler object met snelheid `jumpForce` naar boven bewogen en zal daarna terug vallen, hierbij word `jumpIsReady` false en word er ook een kleine hoeveelheid rewardscore afgetrokken om willekeurig springen wanneer het niet nodig is af te straffen.

In de `FixedUpdate()` functie zal `RequestDecision()` aangeroepen worden als `jumpIsReady` true is, deze zal op basis van de machine learning agent beslissen om te springen of niet, als deze besluit om niet te springen krijgt het een zéér kleine hoeveelheid `rewardScore` om op de grond blijven wanneer er niet gesprongen hoeft te worden aan te moedigen.

Wanneer een object het *Player* object aanraakt word `OnCollisionEnter(Collider collision)` aangeroepen, als dit een *Obstacle* object is verliest de agent een grote hoeveelheid rewardscore en word het object vernietigd, als het het *Floor* object is word `jumpIsReady` opnieuw True.

Aangezien *Coins* trigger colliders hebben word hiervoor de functie `OnTriggerEnter(COllider collision)` aangeroepen, hierbij word de *Coin* vernietigd en krijgt de agent een grote hoeveelheid rewardscore.

### *Coin*

Het *Coin* script bevat de floats `givenSpeed`, `randomizedSpeed`, `maxSpeed` en `minSpeed`, alsook de boolean `constantGivenSpeed`, gebaseerd op deze boolean word het `randomizedSpeed` in de `Start()` functie een vaste waarde givenSpeed of een willekeurige waarde tussen `maxSpeed` en `minSpeed` gegeven. Ook heeft het script een *Environment* object om objecten uit het *Environment* te kunnen ophalen.

*Coin* heeft ook een `FixedUpdate()` functie, waarin de `Move()` functie aangeroepen word als `randomizedSpeed` hoger dan 0 is, `Move()` beweegt de *Coin* in zijn forward richting aan snelheid `randomizedSpeed * Time.deltaTime`.

Ook heeft *Coin* een `OnCollisionEnter(Collider other)` functie, deze evalueert of de Collider die het aanraakt die van een *DeadLine* is. Als dat zo is dan word de *Coin* verwijderd en verliest de agent een kleine hoeveelheid rewardscore.

### *Obstacle*

Het *Obstacle* script is synoniem aan het *Coin* script, behalve dat deze de agent een grote hoeveelheid rewardscore geeft bij het bereiken van een *DeadLine* object, de givenSpeed waarde verschilt ook zodat er een verschil is tussen de *Coin* en *Obstacle* objecten.

## Scènes

Voor een duidelijke afbakening te maken tussen de testomgeving van de verkregen breinen de Intelligence Agents en de leeromgeving van deze agents, zijn er twee scènes gemaakt.

### *SampleScene*

![*SampleScene*](https://i.imgur.com/MVnF3wY.png)

In *SampleScene* scène zal er slechts een *Environment* getoond worden waar de verkregen brein van de Intelligence Agents op de (enige) *Player* gezet zal worden. Dit wordt gedaan om de aandacht zo op één specifieke *Environment* te kunnen leggen en er een makkelijke switch gemaakt kan worden tussen het leren en testen. Ook zorgt dit natuurlijk voor veel minder resource (CPU- & GPU-) verbruik.

### *ML Scene*

![*ML Scene*](https://i.imgur.com/GK5SZg5.png)

Op de *ML Scene* worden er tien *Environment*s geplaatst. Wij hebben voor dit aantal geopteerd aangezien dit voor een perfecte harmonie zorgt tussen het CPU- en GPU-verbruik.

Op het eerste zicht zou het niet lijken dat zo'n eenvoudig *Environment* veel zou gebruiken. Echter moet er rekening worden gehouden met: dat er per **x** aantal seconden nieuwe objecten aangemaakt en verwijderd worden, elke *Player* zijn eigen *RayPerceptionSensoren* wat zeer CPU-intensief is, etc.
