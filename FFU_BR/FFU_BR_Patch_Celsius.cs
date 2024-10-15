#pragma warning disable CS0108
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
    public class patch_NumberModule : NumberModule {
        public override void SetData(CondOwner co) {
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
                    if (FFU_BR_Defs.InfoCelsiusKelvin && cond.strName == "StatGasTemp") {
                        double amount = cond.fCount * cond.fConversionFactor;
                        strData = amount.ToString("N3") + cond.strDisplayBonus + " | " + (amount - 273.15d).ToString("N1") + "C";
                    }
                    else strData = (cond.fCount * cond.fConversionFactor).ToString("N3") + cond.strDisplayBonus;
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

        public override void OnUpdateUI() {
            if (_numbList.Count == 0) return;
            foreach (NumbElement element in _numbList) {
                Condition cond = DataHandler.GetCond(element.CondName);
                string strData;
                if (FFU_BR_Defs.InfoCelsiusKelvin && cond.strName == "StatGasTemp") {
                    double amount = _co.GetCondAmount(element.CondName) * cond.fConversionFactor;
                    strData = amount.ToString("N3") + cond.strDisplayBonus + " | " + (amount - 273.15d).ToString("N1") + "C";
                } 
                else strData = (_co.GetCondAmount(element.CondName) * cond.fConversionFactor).ToString("N3") + cond.strDisplayBonus;
                element.SetData(cond.strNameFriendly, element.CondName, strData, cond.strDesc, DataHandler.GetColor(cond.strColor));
            }
            LayoutRebuilder.MarkLayoutForRebuild(base.transform.parent.GetComponent<RectTransform>());
        }
    }
}
