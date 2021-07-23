using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sperm : MonoBehaviour {

    public GameObject tail;
    public GameObject body;
    public GameObject head;
    public GameObject ovulo;
    Rigidbody2D rigBody;

    float tailRotation = 0;
    float tailSpeed = 1000f;

    // Cromosomas
    public float tailLongitude = 3f;
    public float spermSize = 1f;
    public float perforation = 2f;

    // Derivate Values
    public float aerodinamic;
    float spermSpeed;    
    public float stamina;
    
    // Movement States
    public enum CharacterState { inProgres, Death, Win };
    public CharacterState actualState = CharacterState.inProgres;

    public enum AmbientFriction { Fluid = 100, OvuleWall = 3 }
    public AmbientFriction actualFriction;
    
    static float ovuleOutDistance = 13.5f;
    static float ovuleInnerDistance = 8.5f;

    //AlgorithmValues
    public float ovuleDistance;
    public float time;
    public bool iSurvive;

    // Start is called before the first frame update
    void Start()
    {
        
        rigBody = GetComponent<Rigidbody2D>();
        
        RestartAtributtesRelation();
        RestartCharacter();
        
    }

    // Update is called once per frame
    void Update()
    {
        ovuleDistance = Vector3.Distance(transform.position, ovulo.transform.position);

        ManualControll();

        StateControll();
        ShowToAim(ovulo.transform);
        if (actualState == CharacterState.inProgres) {
            MoveForward();
        }
        
        TailMovement();

        FluidFriction();
        //RaycastDraw();
        //RestartAtributtesRelation();
    }

    // Control Manual del Esperma
    void ManualControll() {
        if (Input.GetKey(KeyCode.Space)) {
            MoveForward();
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            RightMovement();
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            LeftMovement();
        }
    }

    // Control de la maquina de estados
    void StateControll() {
        if ( actualState == CharacterState.inProgres ) {
            time += Time.deltaTime;
            //Debug.Log("InProgress");
        }
        if (actualState == CharacterState.inProgres && ovuleDistance < ovuleInnerDistance) {
            actualState = CharacterState.Win;
            //Debug.Log("SpermWIN");
        }
        if (actualState == CharacterState.inProgres && stamina <= 0) {
            actualState = CharacterState.Death;
            //Debug.Log("SpermDeath");
        }
    }

    // Orientacion Constante al Ovulo, tanto para el impulso como para visualizarlo de forma correcta
    void ShowToAim(Transform aim) {
        transform.LookAt(aim);
        transform.Rotate(180, 0, 0);
    }

    // Inicializa los valores principales para reiniciar la partida y recalcula los valores derivados.
    public void RestartAtributtesRelation() {
        actualState = CharacterState.inProgres;
        actualFriction = AmbientFriction.Fluid;
        aerodinamic = perforation / spermSize;
        stamina = spermSize * 1000;
        spermSpeed = tailLongitude * aerodinamic;
        time = 0;
        iSurvive = false;
    }

    // Impulso frontal del esperma
    void MoveForward() {
        if (stamina > 0) {
            rigBody.AddForce((transform.position - tail.transform.position) * spermSpeed , ForceMode2D.Force);
            stamina -= spermSize;
        }

    }

    // Movimiento lateral para el control manual
    void RightMovement() {
        transform.Rotate(-4, 0, 0);
    }
    void LeftMovement() {
        transform.Rotate(4, 0, 0);
    }

    // Calculo del la frenada que ejercen los fluidos del ambiente, y cambio de fluidos o densidades.
    void FluidFriction() {
        rigBody.velocity -= new Vector2(rigBody.velocity.x / ((int)actualFriction * aerodinamic), rigBody.velocity.y / ((int)actualFriction * aerodinamic));

        if (ovuleOutDistance > ovuleDistance) {
            actualFriction = AmbientFriction.OvuleWall;
        }
        else {
            actualFriction = AmbientFriction.Fluid;
        }
        //Debug.Log(rigBody.velocity.magnitude);
        //Debug.Log(rigBody.velocity);
    }

    // Raycast del impulso del esperma
    void RaycastDraw() {
        Debug.DrawRay(transform.position, (transform.position - tail.transform.position)*50 , Color.green);
        //Debug.Log(transform.position - tail.transform.position);
    }

    // Movimiento del Flagelo en funcion del desplazamiento.
    void TailMovement() {
        tailRotation += (tailSpeed * Time.deltaTime * rigBody.velocity.magnitude);
        tail.transform.localRotation = Quaternion.Euler(0, 0, tailRotation);
    }

    // Reestablece la apariencia del esperma
    public void RestartCharacter() {
        tail.transform.localScale = new Vector3( 1f, 1f, tailLongitude);
        head.transform.localScale = new Vector3(spermSize, spermSize, spermSize*perforation);
        body.transform.localScale = new Vector3(spermSize, spermSize, spermSize);
    }

    // Cambio de material para los cambios generacionales
    public void ChangeHeadMaterial(Material mat) {
        head.GetComponent<MeshRenderer>().material = mat;
    }
    public void ChangeBodyMaterial(Material mat) {
        tail.GetComponent<MeshRenderer>().material = mat;
        body.GetComponent<MeshRenderer>().material = mat;
    }
    public void ChangeFullMaterial(Material mat) {
        tail.GetComponent<MeshRenderer>().material = mat;
        body.GetComponent<MeshRenderer>().material = mat;
        head.GetComponent<MeshRenderer>().material = mat;
    }
}
