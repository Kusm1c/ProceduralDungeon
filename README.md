# ProceduralDungeon

    1. Import the project into Unity
    2. Open the scene `Assets/Scenes/SampleScene.unity`
    3. Add the Package `AI Navigation` to the project

## Features
![Alt text](Generator.png?raw=true "Title")

# Générateur
## Terrain Dimensions (zone Rouge) :
    - Défini si on souhaite une génération à partir d’un nombre aléatoire ou d’une seed défini.

## Runtime Dimensions (zone Rose) :
    - Défini la taille de la room ainsi que construire la taille du sol en X et en Y

## Layers Parameters (zone Verte) :
    - Défini les éléments qui vont être pris dans le calcul de la génération de la room (voir Multiple Rooms, voir parti Multi Room)

## Seed Parameters (zone Bleu) :
    - Défini si on souhaite une génération à partir d’un nombre aléatoire ou d’une seed défini.

## Runtime Parameters (zone Rose) :
    - Permet de modifier les paramètres appliquer à la génération en temps réel (Exemple : Modifier la taille X ou Y) que ce soit en prévisualisation 2D comme en génération 3D.

## Fonctionnalité des différents boutons  :
### 1. Generate Terrain (Jaune Pale) :
    - Créer le terrain à partir des dimensions mise dans Terrains Dimensions. Le mesh est généré pour faire la taille du sol. De plus, son matérial possède un shader adaptant le tiling de la texture à la taille du terrain.
### 2. Generate Terrain Data (Vert Saturé) :
    - Créer les données à partir des layers. Cela va permettre de créer une previsualisation 2D des futurs éléments 3D et de reconnaitre leurs types.
### 3. Update Terrain Matérial (Rose) :
    - Permet de mettre à jour le matérial si besoin. (Fonctionnalité à mettre à jour pour afficher une grid).
### 4. Build NavMesh (Bleu Foncé) :
    - Permet de concevoir et de créer le Navmesh à partir des dimensions et en prenant compte des informations présent dans Layers.
### 5. Cook  3D World (Jaune Chaud) :
    - Créer la visualisation des meshs à partir des informations de layers 2D.
### 6. Clear World (Violet Foncé) :
    - Permet de recréer un Terrain de 0 en supprimant le terrain précédent.

### Door Parameters (zone Vert clair) :
    - Défini les portes de sortie de la Room.
    ⚠️ Attention, si la Door 3D n’est pas setup, lors du Cook, le prefab ne sera pas render.
    - Option permettant d’avoir plusieurs portes du même côté.
    - 🔨 WIP 🔨=> Potentiel point d’amélioration du tools.

### Multi Room Parameters (zone Jaune clair) :
    - Permet de définir si on souhaite avoir plusieurs rooms. Si oui leurs nombres et une taille minimale et maximale.
    - L’offset Seed est une « clé » qui est ajouté dans la seed du générator et permet d’obtenir une génération différente selon la salle.
    - 🔨 WIP 🔨=> List des sizes minimales et maximales en fonction du numéro de la salle.

### Shader Parameters (zone Bleu clair) :
    -	🔨 WIP 🔨=> Permet de display une grid en 2D et de contrôler l’épaisseur des lignes.
