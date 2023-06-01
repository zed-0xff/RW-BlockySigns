using Verse;

namespace Blocky.Signs;

public class Dialog_Rename : Verse.Dialog_Rename {
    private CompNameable nameable;

    public Dialog_Rename(CompNameable nameable) {
        this.nameable = nameable;
        curName = nameable.Name;
    }

    protected override AcceptanceReport NameIsValid(string name) {
        return true;
    }

    protected override void SetName(string name) {
        nameable.Name = name;
    }
}
