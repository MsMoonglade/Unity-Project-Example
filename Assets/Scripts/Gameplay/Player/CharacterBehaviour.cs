using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using Unity.Mathematics;
using System.Text;
using UnityEditor;
using Lofelt.NiceVibrations;
using TMPro;

public class CharacterBehaviour : MonoBehaviour
{
    public static CharacterBehaviour instance;

    [Header ("Variables")]
    public float moveSpeed;
    public float horMovesped;
    public float jumpHeight;
    public float jumpSpeed;
    public float towerColJumpHeight;
    public float towerColJumpSpeed;

    [Header("Fixed Variables")]
    public float moveXLimit;
    public float bulletActiveTime;
    public float moneyDecalScaleX;
    public float moneyDecalScaleY;
    public Vector3 editPosition;
    public Vector3 editRotation;
    public Vector3 editScale;
    public float editAnimSpeed;

    [Header ("Project References")]
    public GameObject bullet_Pref;
    //Edit Stuff
    public GameObject editBG;
    public GameObject editObject;

    [Header ("Local References")]
    public GameObject model;
    public GameObject handler;
    public GameObject printerObject;
    public ParticleSystem powerUpParticle;
    public ParticleSystem powerDownParticle;
    //Edit Stuf
    public GameObject editObjectParent;
    public GameObject editBGParent;
    public CharacterEditGrid characterGrid;
    //ui stuff
    public CanvasGroup characterGamePalenCanvasG;
    public TMP_Text characterGameValueText;

    [Header ("Private References")]     
    private Coroutine jumpCoroutine;
    private PlayerSave save;
    private bool spreadShoot;

    [Header("Private Variables")]
    private Vector3 startPos;
    private quaternion startRot;
    private Vector2 printerObjectScale;    

    private List<int> gridElementValue = new List<int>();
    private List<Vector3> gridElementPos = new List<Vector3>();
    [HideInInspector]
    public List<PlayerMoneyObject> playerMoneyObjects = new List<PlayerMoneyObject> ();

    private void Awake()
    {
        instance = this;

        jumpCoroutine = null;

        save = transform.GetComponent<PlayerSave>();

        startPos = transform.position;
        startRot = transform.rotation;

        Vector3 startScale = new Vector3(0, 1, 0);

        if (PlayerPrefs.HasKey("Xsize"))
            startScale.x = PlayerPrefs.GetFloat("Xsize");
        else
        {
            startScale.x = 3f;
            PlayerPrefs.SetFloat("Xsize", startScale.x);
        }

        if (PlayerPrefs.HasKey("Zsize"))
            startScale.z = PlayerPrefs.GetFloat("Zsize");
        else
        {
            startScale.z = 0.75f;
            PlayerPrefs.SetFloat("Zsize", startScale.z);
        }

        //JumpSped
        if (PlayerPrefs.HasKey("JumpSpeed"))
            jumpSpeed = PlayerPrefs.GetFloat("JumpSpeed");
        else
        {            
            PlayerPrefs.SetFloat("JumpSpeed", jumpSpeed);
        }

        
        //MoveSpeed
        if (PlayerPrefs.HasKey("MoveSpeed"))
            moveSpeed = PlayerPrefs.GetFloat("MoveSpeed");
        else
        {
            PlayerPrefs.SetFloat("MoveSpeed", moveSpeed);
        }        

        printerObject.transform.localScale = startScale;
        printerObjectScale = new Vector2(printerObject.transform.localScale.x, printerObject.transform.localScale.z);

        characterGamePalenCanvasG.alpha = 0;
        spreadShoot = false;

        LoadPlayerValue();
    }

    private void Start()
    {
        foreach (GameObject o in characterGrid.currentGridElement)
        {
            int myValue = 0;           

            for (int i = 0; i < gridElementPos.Count; i++)
            {
                if(o.transform.localPosition == gridElementPos[i])
                {
                    myValue = gridElementValue[i];
                    break;
                }
            }

            GameObject edit = Instantiate(editObject, o.transform.position, o.transform.rotation, editObjectParent.transform);
            playerMoneyObjects.Add(edit.GetComponent<PlayerMoneyObject>());
            edit.GetComponent<PlayerMoneyObject>().Setup(myValue);

            //BG
            GameObject bg = Instantiate(editBG, o.transform.position + new Vector3(0 , 0.05f , 0), o.transform.rotation, editBGParent.transform);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.playGame, OnPlayGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.playGame, OnPlayGame);
    }

    private void Update()
    {
        //MOVE
        if (GameManager.instance.IsInGameStatus())
        {
            //move forward if is in game
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            //clamp Character X Position
            transform.position = new Vector3(Mathf.Clamp(CharacterBehaviour.instance.transform.position.x, -moveXLimit, moveXLimit), CharacterBehaviour.instance.transform.position.y, CharacterBehaviour.instance.transform.position.z);   
        }

        characterGameValueText.text = "$" + CalculatePlayerPrintPerSecond().ToString() + "/Sec";
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Wall"))
        {
            col.transform.GetComponent<WallBehaviour>().Take();
        }

        if (col.transform.CompareTag("Tower"))
        {
            TowerColFeedback();
            col.transform.GetComponent<RewardTower>().HitPlayer();                
        }

        if (col.transform.CompareTag("EndMiniGame"))
        {
            OnEnterEndGame();
        }
    }

    public void Move(Vector3 direction)
    {
        if(GameManager.instance.IsInGameStatus())       
            transform.Translate(direction * Time.deltaTime * horMovesped) ;
    }

    public void SetSpreadShoot()
    {
        if(!spreadShoot)
            spreadShoot = true;
    }

    public void SetInEditPos()
    {
        transform.DOMove(editPosition, 0);
        transform.DORotate(editRotation, 0);
        transform.DOScale(editScale, 0);            
    }

    public void ConfirmEdit()
    {
        transform.DOMove(startPos, editAnimSpeed);
        transform.DORotateQuaternion(startRot, editAnimSpeed)
            .OnComplete(()=> GameManager.instance.inEdit = false);
        transform.DOScale(1, editAnimSpeed);

        Sequence delayUiActivation = DOTween.Sequence();
        delayUiActivation.PrependInterval(editAnimSpeed);
        delayUiActivation.Append(characterGamePalenCanvasG.DOFade(1 , editAnimSpeed/2));
    }

    public int TotalCharacterValue()
    {
        int o_TotalValue = 0;

        for (int i = 0; i < playerMoneyObjects.Count; i++)
        {           
           o_TotalValue += playerMoneyObjects[i].value;
        }

        return o_TotalValue;
    }

    private void ShootMoney()
    {
        if (DeviceCapabilities.isVersionSupported)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);         
        }

        for (int i = 0; i < playerMoneyObjects.Count; i++)
        {
            //shoot if character is moving forward(not back by feedback)
            if(moveSpeed > 0)            
                playerMoneyObjects[i].Shoot(spreadShoot);
        }

        //Jump Anim
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(handler.transform.DOScaleY(0.7f, jumpSpeed / 2));
        mySequence.Append(handler.transform.DOScaleY(1.2f, jumpSpeed / 2));        
    }

    public void ApplyPrinterScale(Vector3 newScale, bool right)
    {
        printerObject.transform.DOScale(newScale, 0.5f)
            .SetEase(Ease.OutSine);

        PlayerPrefs.SetFloat("Xsize", newScale.x);
        PlayerPrefs.SetFloat("Zsize", newScale.z);

        printerObjectScale = new Vector2(newScale.x, newScale.z);

        for (int i = 0; i < gridElementPos.Count; i++)
        {
            Vector3 fixedPos = gridElementPos[i];

            if (right)
                fixedPos += new Vector3(-0.75f, 0, 0);

            else
                fixedPos += new Vector3(0, 0, 0.75f/2);

            gridElementPos[i] = fixedPos;
        }

        foreach(PlayerMoneyObject o in playerMoneyObjects)
        {
            Vector3 fixedPos = o.transform.localPosition;

            if (right)
                fixedPos += new Vector3(-0.75f, 0, 0);

            else
                fixedPos += new Vector3(0, 0, 0.75f / 2);

            o.transform.DOLocalMove(fixedPos, 0.5f);
        }

        foreach (Transform o in editBGParent.transform)
        {
            o.transform.DOScale(Vector3.zero, 0.2f);
        }

        Invoke("RegenerateGridSlotAfterSizeChange", 0.55f);
    }

    private void RegenerateGridSlotAfterSizeChange()
    {
        characterGrid.UpdateSlotWithNewSize();
       
        foreach (GameObject o in characterGrid.currentGridElement)
        {
            GameObject bg = Instantiate(editBG, o.transform.position + new Vector3(0, 0.05f, 0), o.transform.rotation, editBGParent.transform);
            bg.transform.localPosition = o.transform.localPosition;
            bg.transform.localScale = Vector3.zero;
            bg.transform.DOScale(Vector3.one, 0.2f);
        }

        SavePlayerValue();
    }

    private int CalculatePlayerPrintPerSecond()
    {
        int o_pps = 0;

        o_pps = (int)(TotalCharacterValue() / jumpSpeed);

        if (spreadShoot)
            o_pps *= 2;

        return o_pps;
    }

    private void OnEnterEndGame()
    {   
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }
        transform.DOPause();
        transform.DOKill();

        EndGameBehaviour.instance.StartEndGame();
        moveSpeed = 0;
    }

    private IEnumerator JumpCoroutine()
    {
        while (GameManager.instance.IsInGameStatus())
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(model.transform.DOLocalMoveY(transform.localPosition.y + jumpHeight, jumpSpeed/2)               
                .SetEase(Ease.OutSine));

            mySequence.Append(model.transform.DOLocalMoveY(0, jumpSpeed / 2)                          
                .SetEase(Ease.InSine)
                .OnComplete(ShootMoney));

            yield return new WaitForSeconds(jumpSpeed + 0.05f);
        }
    }

    private void TowerColFeedback()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }

        transform.DOPause();
        transform.DOKill();

        moveSpeed = -moveSpeed;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(model.transform.DOLocalMoveY(towerColJumpHeight, towerColJumpSpeed / 2));        

        mySequence.Append(model.transform.DOLocalMoveY(0, towerColJumpSpeed / 2)
            .SetEase(Ease.InOutSine)
            .OnComplete(TowerColReset));
    }

    private void TowerColReset()
    {
        moveSpeed = math.abs(moveSpeed);

        ShootMoney();

        if(jumpCoroutine == null)
        {
            jumpCoroutine = StartCoroutine(JumpCoroutine());
        }
    }

    private void OnPlayGame(object sender)
    {
        if (jumpCoroutine == null)
            jumpCoroutine = StartCoroutine(JumpCoroutine());
    }

    public void EditFireRateInLocalGame(float amount)
    {
        jumpSpeed -= amount;

        if (jumpSpeed <= 0.15f)
            jumpSpeed = 0.15f;

        if(amount > 0)      
            powerUpParticle.Play();
       else
            powerDownParticle.Play();
    }

    public void IncreaseJumpSpeed(float increaseAmount)
    {
        powerUpParticle.Play();

        jumpSpeed -= increaseAmount;
        
        if (jumpSpeed <= 0.15f)
            jumpSpeed = 0.15f;

        PlayerPrefs.SetFloat("JumpSpeed", jumpSpeed);
    }

    public void IncreaseMoveSpeed(float increaseAmount)
    {
        powerUpParticle.Play();

        moveSpeed += increaseAmount;

        if (moveSpeed > 12f)
            moveSpeed = 12f;

        PlayerPrefs.SetFloat("MoveSpeed", moveSpeed);
    }

    public void SavePlayerValue()
    {       
        int[] valueToSave = new int[editObjectParent.transform.childCount];
        Vector3[] posToSave = new Vector3[editObjectParent.transform.childCount];

        for(int i = 0; i < editObjectParent.transform.childCount; i++)
        {
            valueToSave[i] = editObjectParent.transform.GetChild(i).GetComponent<PlayerMoneyObject>().value;
            posToSave[i] = editObjectParent.transform.GetChild(i).transform.localPosition;
        }

        PlayerPrefs.DeleteKey("SavedValue");
        PlayerPrefs.SetString("SavedValue", string.Join("###", valueToSave));

        string posToSaveString = SerializeVector3Array(posToSave);
        PlayerPrefs.DeleteKey("SavedPos");
        PlayerPrefs.SetString("SavedPos", posToSaveString);

        LoadPlayerValue();
    }

    public void LoadPlayerValue()
    {
        int xQuantity = (int)(printerObject.transform.localScale.x / moneyDecalScaleX);
        int yQuantity = (int)(printerObject.transform.localScale.z / moneyDecalScaleY);

        gridElementPos.Clear();
        gridElementValue.Clear();

        if (PlayerPrefs.HasKey("SavedValue") && PlayerPrefs.HasKey("SavedPos"))
        {
            //LoadValue
            string[] tempValue = PlayerPrefs.GetString("SavedValue").Split(new[] { "###" }, StringSplitOptions.None);

            if (tempValue[0] != "")
            {
                if (tempValue.Length >= 1)
                    for (int i = 0; i < tempValue.Length; i++)
                    {
                        gridElementValue.Add(int.Parse(tempValue[i]));
                    }

                //LoadPos
                string posStringNotSplitted = PlayerPrefs.GetString("SavedPos");
                Vector3[] allPosSplitted = DeserializeVector3Array(posStringNotSplitted);
                if (allPosSplitted.Length >= 1)
                    for (int i = 0; i < allPosSplitted.Length; i++)
                    {
                        gridElementPos.Add(allPosSplitted[i]);
                    }
            }
        }

        else
        {
            Vector3 startPoint = new Vector3(          
                printerObject.transform.position.x - (printerObjectScale.x / 2) + (moneyDecalScaleX / 2),          
                0,          
                printerObject.transform.position.z - (printerObjectScale.y / 2) + (moneyDecalScaleY / 2));

            List<int> tempValue = new List<int>();
            List<float> tempX = new List<float>();
            List<float> tempZ = new List<float>();

            for (int i = 0; i < xQuantity; i++)
            {
                for (int j = 0; j < yQuantity; j++)
                {
                    tempValue.Add(1);
                    tempX.Add(startPoint.x);
                    tempZ.Add(startPoint.z);

                    startPoint += new Vector3(0, 0, moneyDecalScaleY);
                }

                startPoint += new Vector3(moneyDecalScaleX, 0, (-yQuantity * moneyDecalScaleY));
            }

            int[] valueToSave = new int[tempValue.Count];
            Vector3[] posToSave = new Vector3[tempX.Count];

            for (int i = 0; i < valueToSave.Length; i++)
            {
                valueToSave[i] = tempValue[i];
                gridElementValue.Add(tempValue[i]);

                posToSave[i] = new Vector3(tempX[i], -0.1f, tempZ[i]);
                gridElementPos.Add(posToSave[i]);
            }

            string posToSaveString = SerializeVector3Array(posToSave);          

            PlayerPrefs.SetString("SavedValue", string.Join("###", valueToSave));
            PlayerPrefs.SetString("SavedPos", posToSaveString);
        }
    }

    public static string SerializeVector3Array(Vector3[] aVectors)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 v in aVectors)
        {
            sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
        }
        if (sb.Length > 0) // remove last "|"
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    public static Vector3[] DeserializeVector3Array(string aData)
    {
        string[] vectors = aData.Split('|');
        Vector3[] result = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = vectors[i].Split(' ');
            if (values.Length != 3)
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
            result[i] = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        return result;
    }
}