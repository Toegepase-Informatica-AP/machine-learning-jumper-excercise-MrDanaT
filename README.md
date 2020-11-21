# machine-learning-jumper-excercise-MrDanaT

## Intro
In dit Unity project leert een AI om over obstakels te springen door gebruik van een beloning systeem met munten, deze ReadMe legt uit welke verschillende objecten en scripts actief zijn en hoe zij werken.

## Objecten

### ScoreBoard
ScoreBoard is een text display dat de huidige rewardscore laat zien.

### Player
Het Player object word voorgesteld door een simpele bean met een gezichtje, dit object staat stil op het veld en heeft enkel de "jump" actie, die het uit vrije wil kan nemen gebaseerd op machine learning. Het heeft een collider die gebruikt word in interacties met Floor, Munt, Coin en Obstacle. Ook heeft het een "LeftEye" en "RightEye" child object die de ogen voorstellen en met Ray Perception Sensors ervoor zorgen dat het de Coin en Obstacle objecten kan zien.

Door het Floor object aan te raken weet de agent dat het opnieuw kan springen, de agent krijgt rewardscore wanneer Player een Coin aanraakt en verliest rewardscore als Player een Obstacle aanraakt

### Floor
Het Floor object is een simpele vloer waar de andere objecten op "staan" en overheen gaan, het heeft zelf geen gedrag om uit te voeren. Het heeft een collider die gebruikt word in interactes met Player

### SpawnLine 
Het SpawnLine object is een lijn die het einde van de vloer markeert en gebruikt word om objecten te spawnen, het heeft van zichzelf geen gedrag.

### DeadLine
Het DeadLine object is een lijn die het einde van de vloer markeert en gebruikt word om objecten de despawnen en daarbij de rewardscore aan te passen.

### Coins
Een parent object voor de Coin objecten, gebruikt door het environment script.

### Coin
Het Coin object is een munt die bij SpawnLine verschijnt en in een lijn naar de DeadLine (en de speler) beweegt, de collider word gebruikt in interacties met Player en DeadLine.

Als een Coin het Player object aanraakt krijgt de agent rewardscore, als het het DeadLine object aanraakt verliest de agent rewardscore.

### Obstacles
Een Parent object voor de Obstacle object, gebruikt door het environement script.

### Obstacle
Het Obstacle object is een balk die bij SpawnLine verschijnt en in een lijn naar de DeadLine beweegt, de collider word gebruikt in interacties met Speler en Deadline.

Als een Obstacle het DeadLine object aanraakt krijgt de agent rewardscore, als het het Player object aanraakt verliest de agent rewardscore.

## Scripts
### Environment
Het environment script is verantwoordelijk voor het instantiëren van de verschillende objecten en voor het aanmaken van Coin en Obstacle objecten op een semi-willekeurige wijze.

Dit script bevat private objectinstanties van alle objecten in het project, zodat deze makkelijk toegankelijk zijn voor andere scripts, alsook de floats maxTime, minTime, currentTimer en randomTimed.

In de Start() functie begint currentTimer van waarde 0 en krijgt randomTimed een waarde tussen maxTime en minTime.

In de OnEnable() functie worden alle private variabelen zoals coins geinstantieërd.

Ook bevat dit script de functies ClearEnvironment() om Coins en Obstacles de verwijderen en het bord te resetten.

De functies SpawnCoin() en SpawnObstacle() worden gebruikt om currentTimer te resetten en vervolgens het corresponderend object te spawnen op de locatie van het SpawnLine object met de correcte orientatie.

het script bevat ook een FixedUpdate() functie, in deze functie word currentTimer incrementeel verhoogd word er een randomValue tussen een kleine range(3) gegenereerd, als randomTimed kleiner of gelijk is aan currentTimer kan een object spawnen, dit gebeurt afhankelijk van de randomValue; bij 1 spawnt een Coin, bij 2 een Obstacle en bij 3 gebeurt er niets behalve een reset van currentTimer. Op deze manier is het gedrag van het environment semi-willekeurig.

### Player
Het Player script bevat de float jumpForce die uitmaakt hoe hoog het object zal springen alsook de bool jumpIsReady. Ook heeft het script een Environment object om objecten uit het Environment te kunnen ophalen en zijn eigen Rigidbody object.

In de Jump() functie word het speler object met snelheid jumpForce naar boven bewogen en zal daarna terug vallen, hierbij word jumpIsReady false en word er ook een kleine hoeveelheid rewardscore afgetrokken om willekeurig springen wanneer het niet nodig is af te straffen.

In de FixedUpdate() functie zal RequestDecision() aangeroepen worden als jumpIsReady true is, deze zal op basis van de machine learning agent beslissen om te springen of niet, als deze besluit om niet te springen krijgt het een zéér kleine hoeveelheid rewardScore om op de grond blijven wanneer er niet gesprongen hoeft te worden aan te moedigen.

Wanneer een object het Player object aanraakt word OnCollisionEnter(Collider collision) aangeroepen, als dit een Obstacle object is verliest de agent een grote hoeveelheid rewardscore en word het object vernietigd, als het het Floor object is word jumpIsReady opnieuw True.

Aangezien Coins trigger colliders hebben word hiervoor de functie OnTriggerEnter(COllider collision) aangeroepen, hierbij word de Coin vernietigd en krijgt de agent een grote hoeveelheid rewardscore.

### Coin
Het Coin script bevat de floats givenSpeed, randomizedSpeed, gaxSpeed en gaxSpeed, alsook de boolean constantGivenSpeed, gebaseerd op deze boolean word het randomizedSpeed in de Start() functie een vaste waarde givenSpeed of een willekeurige waarde tussen MaxSpeed en MinSpeed gegeven. Ook heeft het script een Environment object om objecten uit het Environment te kunnen ophalen.

Coin heeft ook een FixedUpdate() functie, waarin de Move() functie aangeroepen word als randomizedSpeed hoger dan 0 is, Move() beweegt de coin in zijn forward richting aan snelheid randomizedSpeed * Time.deltaTime.

Ook heeft Coin een OnCollisionEnter(Collider other) functie, deze evalueert of de Collider die het aanraakt die van een DeadLine is, als dat zo is dan word de Coin verwijdert en verliest de agent een kleine hoeveelheid rewardscore.

### Obstacle
Het Obstacle script is synoniem aan het Coin script, behalve dat deze de agent een grote hoeveelheid rewardscore geeft bij het bereiken van een DeadLine object, de givenSpeed waarde verschilt ook zodat er een verschil is tussen de Coin en Obstacle objecten.

