#pragma warning disable CS0108
#pragma warning disable CS0114
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using FFU_Beyond_Reach;
using Ostranauts.UI.MegaToolTip.DataModules.SubElements;
using UnityEngine;
using UnityEngine.UI;

namespace Ostranauts.UI.MegaToolTip.DataModules {
    public partial class patch_NumberModule : NumberModule {
        public void SetData(CondOwner co) {
            if (co == null || co.mapConds == null) {
                _IsMarkedForDestroy = true;
                return;
            }
            _numbList.Clear();
            _co = co;
            int num = 0;
            foreach (Condition cond in co.mapConds.Values) {
                if (cond.nDisplayType == 1) {
                    NumbElement component = Object.Instantiate(_numberElement, _tfNumbContainer.transform).GetComponent<NumbElement>();
                    string strData;
                    if (FFU_BR_Defs.AltTempEnabled && cond.strName == "StatGasTemp") {
                        double amount = cond.fCount * cond.fConversionFactor;
                        double altAmount = cond.fCount * FFU_BR_Defs.AltTempMult + FFU_BR_Defs.AltTempShift;
                        strData = amount.ToString("N3") + cond.strDisplayBonus + " | " + altAmount.ToString("N1") + FFU_BR_Defs.AltTempSymbol;
                    } else strData = (cond.fCount * cond.fConversionFactor).ToString("N3") + cond.strDisplayBonus;
                    component.SetData(cond.strNameFriendly, cond.strName, strData, cond.strDesc, DataHandler.GetColor(cond.strColor));
                    _numbList.Add(component);
                    num++;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                    component.ForceMeshUpdate();
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.parent.GetComponent<RectTransform>());
            if (num == 0) {
                _IsMarkedForDestroy = true;
            }
        }
        protected void OnUpdateUI() {
            if (_numbList.Count == 0) return;
            foreach (NumbElement element in _numbList) {
                Condition cond = DataHandler.GetCond(element.CondName);
                string strData;
                double condAmount = _co.GetCondAmount(element.CondName);
                if (FFU_BR_Defs.AltTempEnabled && cond.strName == "StatGasTemp") {
                    double amount = condAmount * cond.fConversionFactor;
                    double altAmount = condAmount * FFU_BR_Defs.AltTempMult + FFU_BR_Defs.AltTempShift;
                    strData = amount.ToString("N3") + cond.strDisplayBonus + " | " + altAmount.ToString("N1") + FFU_BR_Defs.AltTempSymbol;
                } else strData = (condAmount * cond.fConversionFactor).ToString("N3") + cond.strDisplayBonus;
                element.SetData(cond.strNameFriendly, element.CondName, strData, cond.strDesc, DataHandler.GetColor(cond.strColor));
            }
            LayoutRebuilder.MarkLayoutForRebuild(base.transform.parent.GetComponent<RectTransform>());
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
public override void SetData(CondOwner co)
{
	if (co == null || co.mapConds == null)
	{
		_IsMarkedForDestroy = true;
		return;
	}
	_numbList.Clear();
	_co = co;
	int num = 0;
	foreach (Condition value in co.mapConds.Values)
	{
		if (value.nDisplayType == 1)
		{
			NumbElement component = Object.Instantiate(_numberElement, _tfNumbContainer.transform).GetComponent<NumbElement>();
			string strData = (value.fCount * (double)value.fConversionFactor).ToString("N3") + value.strDisplayBonus;
			if (value.strName == "StatGasTemp")
			{
				strData = MathUtils.GetTemperatureString(value.fCount * (double)value.fConversionFactor);
			}
			component.SetData(value.strNameFriendly, value.strName, strData, value.strDesc, DataHandler.GetColor(value.strColor));
			_numbList.Add(component);
			num++;
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
			component.ForceMeshUpdate();
		}
	}
	LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
	LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.parent as RectTransform);
	if (num == 0)
	{
		_IsMarkedForDestroy = true;
	}
}

protected override void OnUpdateUI()
{
	if (_numbList.Count == 0)
	{
		return;
	}
	foreach (NumbElement numb in _numbList)
	{
		Condition cond = DataHandler.GetCond(numb.CondName);
		string strData = (_co.GetCondAmount(numb.CondName) * (double)cond.fConversionFactor).ToString("N3") + cond.strDisplayBonus;
		if (cond.strName == "StatGasTemp")
		{
			strData = MathUtils.GetTemperatureString(_co.GetCondAmount(numb.CondName) * (double)cond.fConversionFactor);
		}
		numb.SetData(cond.strNameFriendly, numb.CondName, strData, cond.strDesc, DataHandler.GetColor(cond.strColor));
	}
	LayoutRebuilder.MarkLayoutForRebuild(base.transform.parent as RectTransform);
}

*/