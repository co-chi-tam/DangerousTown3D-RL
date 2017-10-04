using System;

[Serializable]
public class QLearnAction {

	public QLearnAction(Action action) :
	this(action.Method.Name, action) {
	}

	public QLearnAction(string name, Action action) {
		this.Name = name;
		this.Action = action;
	}

	public string Name { get; set; }
	public Action Action { get; set; }

	public override string ToString() {
		return string.Format("[QLearnAction: Name={0}]", Name);
	}

	public override bool Equals(object rhs) {
		QLearnAction rhsAction = rhs as QLearnAction;

		if (rhsAction != null) {
			return Name.Equals(rhsAction.Name);
		}

		return false;
	}

	public override int GetHashCode() {
		return Name.GetHashCode();
	}

}