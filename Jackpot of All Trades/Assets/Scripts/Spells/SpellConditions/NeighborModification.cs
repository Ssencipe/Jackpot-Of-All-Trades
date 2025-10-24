using UnityEngine;

[System.Serializable]
public class NeighborModification
{
    public NeighborScope scope;
    public NeighborModificationType type;
    public int amount = 1;

    public void Apply(SpellCastContext context, BaseSpell sourceSpell)
    {
        var neighbors = GridManager.GetSpellsInScope(context, sourceSpell, scope);
        Debug.Log($"[NeighborModification] Applying {type} to {neighbors.Count} neighbors of {sourceSpell.spellData.spellName}");

        foreach (var neighbor in neighbors)
        {
            if (neighbor?.runtimeSpell == null || neighbor == sourceSpell) continue;

            var rs = neighbor.runtimeSpell;
            Debug.Log($"[Before] {neighbor.spellData.spellName} - Charge: {rs.charge}, Tally: {rs.tally}, Potency: {rs.potencyMultiplier}");

            switch (type)
            {
                case NeighborModificationType.ChangePotency:
                    rs.potencyMultiplier += amount;
                    rs.wasPotencyModified = true;
                    break;
                case NeighborModificationType.TakePotency:
                    int p = Mathf.Min(amount, Mathf.FloorToInt(rs.potencyMultiplier));
                    rs.potencyMultiplier -= p;
                    rs.wasPotencyModified = true;
                    sourceSpell.runtimeSpell.potencyMultiplier += p;
                    sourceSpell.runtimeSpell.wasPotencyModified = true;
                    break;
                case NeighborModificationType.ChangeCharge:
                    rs.charge += amount;
                    break;
                case NeighborModificationType.TakeCharge:
                    int c = Mathf.Min(amount, rs.charge);
                    rs.charge -= c;
                    sourceSpell.runtimeSpell.charge += c;
                    break;
                case NeighborModificationType.ChangeTally:
                    rs.tally += amount;
                    break;
                case NeighborModificationType.TakeTally:
                    int t = Mathf.Min(amount, rs.tally);
                    rs.tally -= t;
                    sourceSpell.runtimeSpell.tally += t;
                    break;
            }

            Debug.Log($"[After] {neighbor.spellData.spellName} - Charge: {rs.charge}, Tally: {rs.tally}, Potency: {rs.potencyMultiplier}");

            // Refresh visual on neighbor's reel when changed
            var reel = GridManager.Instance.linkedReels[neighbor.reelIndex];
            reel?.reelVisual?.RefreshAllVisuals();
        }

        // Refresh visual of source spell visual if it was modified
        var sourceReel = GridManager.Instance.linkedReels[sourceSpell.reelIndex];
        sourceReel?.reelVisual?.RefreshAllVisuals();
    }
}
