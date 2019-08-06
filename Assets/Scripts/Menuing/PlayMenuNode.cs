using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayMenuNode
{
    public string Label;
    public PlayMenuInternalNode Parent;

    public List<PlayMenuNode> Siblings => Parent.Children;

    public PlayMenuNode (string label)
    {
        Label = label;
    }

    public PlayMenuNode GetNextSibling ()
    {
        return Siblings[(Siblings.IndexOf(this) + 1) % Siblings.Count];
    }

    public PlayMenuNode GetPreviousSibling ()
    {
        int loc = Siblings.IndexOf(this) - 1;
        if (loc < 0) loc = Siblings.Count - 1;
        return Siblings[loc];
    }

    public List<PlayMenuNode> GetSurrounding (int range)
    {
        List<PlayMenuNode> output = new List<PlayMenuNode>();
        output.Add(this);

        PlayMenuNode backHead = GetPreviousSibling(), frontHead = GetNextSibling();

        for (int i = 0; i < range; i++)
        {
            output.Insert(0, backHead);
            output.Add(frontHead);

            backHead = backHead.GetPreviousSibling();
            frontHead = frontHead.GetNextSibling();
        }

        return output;
    }
}

public class PlayMenuInternalNode : PlayMenuNode
{
    public List<PlayMenuNode> Children;

    public PlayMenuInternalNode (string label, params PlayMenuNode[] children) : base(label)
    {
        Children = new List<PlayMenuNode>(children);

        foreach (var child in Children)
        {
            child.Parent = this;
        }
    }
}

public class PlayMenuLeafNode : PlayMenuNode
{
    public Action OnSelect;

    public PlayMenuLeafNode (string label, Action onSelect) : base(label)
    {
        OnSelect = onSelect;
    }

    public PlayMenuLeafNode (string label, Action<Mage> onSelect) : base(label)
    {
        OnSelect = () => onSelect(MageSquad.Instance.ActiveMage);
    }
}
