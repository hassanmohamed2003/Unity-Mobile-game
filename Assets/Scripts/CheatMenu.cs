using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheatMenu : MonoBehaviour
{
    public TMP_Text UnlockAllText;
    public TMP_Text NoFailText;
    public TMP_Text DisableRopeText;
    public TMP_Text LockCheckpointText;

    void Start()
    {
        UnlockAllText.text = $"Toggle Unlock All Levels (Current Value: {GameState.UnlockAllLevels})";
        NoFailText.text = $"Toggle No Fail (Current Value: {GameState.NoFail})";
        DisableRopeText.text = $"Toggle Disable Rope Break/Swing (Current Value: {GameState.DisableRope})";
        LockCheckpointText.text = $"Toggle Locked Checkpoint (Current Value: {GameState.LockedCheckpoint})";
    }

    public void ToggleUnlockAllLevels()
    {
        GameState.UnlockAllLevels = !GameState.UnlockAllLevels;
        UnlockAllText.text = $"Toggle Unlock All Levels (Current Value: {GameState.UnlockAllLevels})";
    }

    public void ToggleNoFail()
    {
        GameState.NoFail = !GameState.NoFail;
        NoFailText.text = $"Toggle No Fail (Current Value: {GameState.NoFail})";
    }

    public void ToggleDisableRope()
    {
        GameState.DisableRope = !GameState.DisableRope;
        DisableRopeText.text = $"Toggle Disable Rope Break/Swing (Current Value: {GameState.DisableRope})";
    }

    public void ToggleLockedCheckpoint()
    {
        GameState.LockedCheckpoint = !GameState.LockedCheckpoint;
        LockCheckpointText.text = $"Toggle Locked Checkpoint (Current Value: {GameState.LockedCheckpoint})";
    }
}
