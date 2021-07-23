using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{

    public GameObject sperm;
    GameObject[] spermPull;
    public int pullSize = 90;
    public int generationSurvivors = 50;
    int actualSurvivors = 0;
    public float speedSimulation = 5;

    public float mutationProbability = 0.1f;

    float spermDistance = 50;
    float angleSpermDistribution;

    enum AlgorithmState { Inizialice, GenerationRun, Selection, Finish };
    AlgorithmState algorithmState = AlgorithmState.Inizialice;

    public Material mSperm;
    public Material mSpermChampion;
    public Material mSpermSurvive;
    public Material mSpermDeath;

    //UI Info
    [HideInInspector]
    public int generation = 1;
    [HideInInspector]
    public float winners = 0;
    [HideInInspector]
    public float bestTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = speedSimulation;
        GenerateFirstGeneration();
    }

    // Genera la Pull de Esperma y Crea Cromosomas
    void GenerateFirstGeneration() {
        spermPull = new GameObject[pullSize];
        angleSpermDistribution = 360 / pullSize;
        angleSpermDistribution = angleSpermDistribution * Mathf.Deg2Rad;
        //SpermRecolocation();

        for (int i = 0; i < pullSize; i++) {
            spermPull[i] = Instantiate(sperm, new Vector3(Mathf.Cos(angleSpermDistribution * i) * spermDistance,
                                                            Mathf.Sin(angleSpermDistribution * i) * spermDistance, 0),
                                                            Quaternion.identity);
            AsignRandomSpermValues(spermPull[i]);
        }
    }

    // Randomiza los Cromosomas de Un Esperma
    void AsignRandomSpermValues(GameObject sperm) {
        sperm.GetComponent<Sperm>().ovulo = this.gameObject;
        sperm.GetComponent<Sperm>().tailLongitude = Random.Range(2.0f,4.0f);
        sperm.GetComponent<Sperm>().spermSize = Random.Range(0.75f, 2.0f);
        sperm.GetComponent<Sperm>().perforation = Random.Range(1.0f, 3.0f);
        sperm.GetComponent<Sperm>().RestartAtributtesRelation();
        sperm.GetComponent<Sperm>().RestartCharacter();
    }

    // Coloca el esperma en forma de Helice, para ver el ranking.
    void SpermHelixRecolocation() {
        for (int i = 0; i < pullSize; i++) {
            spermPull[i].transform.position = new Vector3(Mathf.Sin(angleSpermDistribution * i) * spermDistance * i * 0.01f,
                                                            Mathf.Cos(angleSpermDistribution * i) * spermDistance * i * 0.01f,
                                                            0);   
        }
    }

    // Coloca el esperma en un circulo para iniciar la generacion.
    void SpermCicleRecolocation() {
        for (int i = 0; i < pullSize; i++) {
            spermPull[i].transform.position = new Vector3(Mathf.Cos(angleSpermDistribution * i) * spermDistance,
                                                            Mathf.Sin(angleSpermDistribution * i) * spermDistance, 0);
        }
    }

    // Pinta y diferencia los que llegaron a la meta y los que no
    void PaintWinnersAndLosers() {
        for (int i = 0; i < pullSize; i++) {
            if (spermPull[i].GetComponent<Sperm>().actualState == Sperm.CharacterState.Win) {
                spermPull[i].GetComponent<Sperm>().ChangeBodyMaterial(mSpermChampion);
                spermPull[i].GetComponent<Sperm>().ChangeHeadMaterial(mSpermDeath);
            }
            if (spermPull[i].GetComponent<Sperm>().actualState == Sperm.CharacterState.Death) {
                spermPull[i].GetComponent<Sperm>().ChangeFullMaterial(mSpermDeath);
            }
        }
    }

    // Reordena todos los espermas en funcion del tiempo de fecundadion o de la 
    // maxima distancia de llegada en caso de no fecundar.
    void ReorderByClassification() {
        GameObject other;
        int winnersCount = 0;
        //Separamos los ganadores de los perdedores
        for (int i = 0; i < pullSize; i++) {
            if (spermPull[i].GetComponent<Sperm>().actualState == Sperm.CharacterState.Win) {                
                other = spermPull[i];
                spermPull[i] = spermPull[winnersCount];
                spermPull[winnersCount] = other;
                ++winnersCount;
            }
        }

        winners = winnersCount;

        // Los Ganadores los valoramos por el tiempo que tardaron en llegar
        for (int i = 0; i < winnersCount; i++) {
            for (int j = 0; j < winnersCount; j++) {
                if (spermPull[i].GetComponent<Sperm>().time < spermPull[j].GetComponent<Sperm>().time) {
                    other = spermPull[i];
                    spermPull[i] = spermPull[j];
                    spermPull[j] = other;
                }
            }
        }
        // Los perdedores los valoramos en funcion de lo proximos que estuvieron de ganar.
        for (int i = winnersCount; i < pullSize; i++) {
            for (int j = winnersCount; j < pullSize; j++) {
                if (spermPull[i].GetComponent<Sperm>().ovuleDistance < spermPull[j].GetComponent<Sperm>().ovuleDistance) {
                    other = spermPull[i];
                    spermPull[i] = spermPull[j];
                    spermPull[j] = other;
                }
            }
        }
        
    }

    // Consulta si todos los espermatozoides llegaron o murieron
    bool isGenerationFinish() {
        for (int i = 0; i < pullSize; i++) {
            if (spermPull[i].GetComponent<Sperm>().actualState == Sperm.CharacterState.inProgres) {
                return false;
            }
        }
        return true;
    }

    // Devuelve los tiempos de todos los espermas
    void PrintScores() {
        for (int i = 0; i < pullSize; i++) {
            Debug.Log(i + "       " +spermPull[i].GetComponent<Sperm>().time);     
        }
    }

    // Selecciona los mejores de forma que los mejores tienen mas posibilidades de seguir en la NextGen.
    void RandomSelection() {
        float rdm;
        while (actualSurvivors < generationSurvivors) {
            for (int i = 0; i < pullSize; i++) {
                rdm = Random.Range(0, 1 * i);
                if (actualSurvivors < generationSurvivors && spermPull[i].GetComponent<Sperm>().iSurvive == false && rdm == 0) {
                    spermPull[i].GetComponent<Sperm>().iSurvive = true;
                    ++actualSurvivors;
                    spermPull[i].GetComponent<Sperm>().ChangeHeadMaterial(mSpermSurvive);
                }

            }
        }
        //Debug.Log(actualSurvivors);
    }

    // Reinicia los valores del esperma y los recoloca.
    void StartGenerationRun() {
        SpermCicleRecolocation();
        for (int i = 0; i < pullSize; i++) {
            spermPull[i].GetComponent<Sperm>().RestartAtributtesRelation();
            spermPull[i].GetComponent<Sperm>().RestartCharacter();
            // Remove to see Clasification
            //spermPull[i].GetComponent<Sperm>().ChangeFullMaterial(mSperm);
        }
    }

    // Crea un Nuevo Espermatozode en funcion de dos padres, con mezcla y probabilidad de mutacion.
    void CreateSperm(GameObject father1, GameObject father2, GameObject child) {
        // Tail
        if (mutationProbability < Random.Range(0.0f, 1.0f)) {
            child.GetComponent<Sperm>().tailLongitude = Random.Range(2.0f, 4.0f);
        }
        else if (0.5f < Random.Range(0.0f, 1.0f)) {
            child.GetComponent<Sperm>().tailLongitude = father1.GetComponent<Sperm>().tailLongitude;
        }
        else {
            child.GetComponent<Sperm>().tailLongitude = father2.GetComponent<Sperm>().tailLongitude;
        }
        // Size
        if (mutationProbability < Random.Range(0.0f, 1.0f)) {
            sperm.GetComponent<Sperm>().spermSize = Random.Range(0.75f, 2.0f);
        }
        else if (0.5f < Random.Range(0.0f, 1.0f)) {
            child.GetComponent<Sperm>().spermSize = father1.GetComponent<Sperm>().spermSize;
        }
        else {
            child.GetComponent<Sperm>().spermSize = father2.GetComponent<Sperm>().spermSize;
        }
        // Child
        if (mutationProbability < Random.Range(0.0f, 1.0f)) {
            sperm.GetComponent<Sperm>().perforation = Random.Range(1.0f, 3.0f);
        }
        else if (0.5f < Random.Range(0.0f, 1.0f)) {
            child.GetComponent<Sperm>().perforation = father1.GetComponent<Sperm>().perforation;
        }
        else {
            child.GetComponent<Sperm>().perforation = father2.GetComponent<Sperm>().perforation;
        }
        child.GetComponent<Sperm>().iSurvive = true;
        child.GetComponent<Sperm>().ChangeFullMaterial(mSperm);
        //Debug.Log("NewGenerarion");

    }

    // Crea una generacion Nueva, mezclando los espermatozoides mas aptos.
    void CreateNextGen() {
        int father1ID = -1;
        int father2ID = -1;
        int childID = -1;
        // Si no estan todos los hijos creados
        while (actualSurvivors < pullSize) {
            //Buscamos Padres y donde va a estar el Hijo.
            for (int i = 0; i < pullSize; i++) {
                // Si no estan selecionados los dos padres,    es un superviviente                         y lo seleccionamos.
                if ( (father1ID == -1 || father2ID == -1) && spermPull[i].GetComponent<Sperm>().iSurvive && Random.Range(0.0f,1.0f) < 0.1f) {
                    if (father1ID == -1 || father1ID == i) {
                        father1ID = i;
                    }
                    else{
                        father2ID = i;
                    }
                }
                if (childID == -1 && !spermPull[i].GetComponent<Sperm>().iSurvive && Random.Range(0.0f, 1.0f) < 0.1f) {
                    childID = i;
                }
            }

            // Creamos un hijo.
            if (father1ID >= 0 && father2ID >= 0 && childID >= 0) {
                CreateSperm(spermPull[father1ID], spermPull[father2ID], spermPull[childID]);
                father1ID = -1;
                father2ID = -1;
                childID = -1;
                ++actualSurvivors;
                //Debug.Log(actualSurvivors);
            }
         
        }
        
          
    }

    // Reinicia los Atributos de toda la pull
    void RestartPullAtributtes() {
        for (int i = 0; i < pullSize; i++) {
            sperm.GetComponent<Sperm>().RestartAtributtesRelation();
        }
        actualSurvivors = 0;
    }

    // Reinicia algunos parametros que va a tomar el HUD.
    void RefreshInfo() {
         
         ++generation;
         //winners = spermPull[0].GetComponent<Sperm>().time;
         bestTime = spermPull[0].GetComponent<Sperm>().time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGenerationFinish() && algorithmState == AlgorithmState.Inizialice) {
            
            RestartPullAtributtes();
            ReorderByClassification();
            SpermHelixRecolocation();
            PaintWinnersAndLosers();
            RefreshInfo();

            RandomSelection();

            

            CreateNextGen();

            StartGenerationRun();

            
            algorithmState = AlgorithmState.Inizialice;            
        }

        //if (isGenerationFinish() && algorithmState == AlgorithmState.Inizialice) {
        //    ReorderByClassification();
        //    SpermHelixRecolocation();
        //    PaintWinnersAndLosers();
        //    RandomSelection();
        //    StartGenerationRun();
        //    algorithmState = AlgorithmState.Selection;
        //}
    }
}
