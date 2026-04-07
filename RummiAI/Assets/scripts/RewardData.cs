using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRewardData", menuName = "ML-Agents/Reward Data")]
public class RewardData : ScriptableObject
{
    [Header("End rewards")]
    public float winReward = 1.0f; //win
    public float lossPenalty = -1.0f; //loss

    [Header("Time rewards")]
    public float WAWFMDT = -0.3f; //when the agent won't finish moves during turn

    [Header("Intermediate rewards")]

    public float PTPBOT = 0.08f; //puting tile properly between other tiles
    public float PTPCTOT = 0.05f; //puting tile properly close to other tile
    public float PTPOLOROTT= 0.04f; //puting tile properly on left or right of two tiles

    public float PTPOB = 0.02f; //puting tile properly on board
    public float MTP = 0.02f; //moving tile properly
    public float MTPTDP = 0.01f; //moving tile to different position that is not its previous position and it is not first turn
    public float IFTFP = 0.3f; //If first turn is finished properly
    public float ATAHFT = 0.3f; //after the turn the agent has fewer tiles

    [Header("Intermediate penalties")]
    public float PNTNT = -0.03f; //Puting new tile when there are no tiles in hand
    public float TNT = -0.4f; //Taking new tile
    public float TNTPT = -0.2f; //taking back tile, per tile
    //public float PRTBOT = -0.01f; //puting wrong tile between other tiles
    public float PTW = -0.09f; //puting tile wrongly
    public float MDDE = -0.04f; //Moving destination doesn't exist
    public float MDIW = -0.05f; //Moving destination is wrong
    public float TRTTCNBR = -0.075f; //trying removing tile that can't be removed, it is permanently put
    public float DBET = -0.15f; //deleting not existing tile
    public float DT = -0.1f; //deleting tile
    public float UA = -0.15f; //undo actions
    public float UAWNCWM = -0.05f; //undo actions when no changes were made
    public float IFTIMW = -0.2f; //If first turn is made wrongly
    public float WEOTTSAOT = -0.2f; //When at the end of turn, the agent has the same amount of tile like at the beginning
    public float MILWAT = -0.2f; //the map is left wrongly after turn
    public float PFETIH = -0.01f; //penalty for each tile in hand
    public float SP = -0.0005f; //to force agent to make the smallest amount of moves in game, step penalty

    [Header("Curriculum Specific Rewards")] //Training
    public float PTPOB_T = 0.1f; //puting tile properly on board
    public float PTPBOT_T = 0.4f; //puting tile properly between other tiles
    public float PTPCTOT_T = 0.2f; //puting tile properly close to other tile
    public float PTPOLOROTT_T = 0.5f; //puting tile properly on left or right of two tiles
    public float ATAHFT_T = 0.5f; //after the turn the agent has fewer tiles

    public float PTPO_T = 0.02f; //puting tile properly on board
    public float TNT_T = -0.5f; //Taking new tile
    public float PTW_T = -0.1f; //puting tile wrongly
    public float MILWAT_T = -0.6f; //the map is left wrongly after turn
    public float PFETIH_T = -0.02f; //penalty for each tile in hand
    



}