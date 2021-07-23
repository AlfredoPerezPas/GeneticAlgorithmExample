using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    public Text generationUI;
    public Text winnersUI;
    public Text bestTimeUI;
    public GameObject IA;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetValues(  IA.GetComponent<GeneticAlgorithm>().generation,
                    IA.GetComponent<GeneticAlgorithm>().winners,
                    IA.GetComponent<GeneticAlgorithm>().bestTime);
    }

    void SetValues(float generation, float winners, float bestTime) {
        generationUI.text = generation.ToString();
        winnersUI.text = winners.ToString();
        bestTimeUI.text = bestTime.ToString();
    }
}
