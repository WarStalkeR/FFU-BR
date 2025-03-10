using System.Collections.Generic;
using UnityEngine;

namespace Ostranauts.UI.MegaToolTip.DataModules {
    public partial class patch_StatusbarModule : StatusbarModule {
        public override void OnUpdateUI() {
            if (_co == null) return;
            UpdateDamageBar();
            if (_co.HasCond("IsPowerObservable")) {
                double currPwrTotal = 0.0;
                double maxPwrTotal = 0.0;
                double percPwr = 0.0;
                maxPwrTotal += _co.GetCondAmount("StatPowerMax") * _co.GetDamageState();
                currPwrTotal += _co.GetCondAmount("StatPower");
                List<CondOwner> subCOs = _co.Pwr != null && _co.Pwr.ctPowerSource != null ?
                    _co.GetCOs(true, _co.Pwr.ctPowerSource) : _co.objContainer != null ?
                    _co.objContainer.GetCOs(true) : new List<CondOwner>();
                if (subCOs != null && subCOs.Count > 0) {
                    foreach (CondOwner item in subCOs) {
                        if (item != null) {
                            maxPwrTotal += item.GetCondAmount("StatPowerMax") * item.GetDamageState();
                            currPwrTotal += item.GetCondAmount("StatPower");
                        }
                    }
                }
                if (maxPwrTotal != 0.0) percPwr = currPwrTotal / maxPwrTotal;
                if (percPwr > 1.0) percPwr = 1.0;
                string pwrTimeLeft = "-";
                if (_co.mapInfo.TryGetValue("PowerRemainingTime", out pwrTimeLeft))
                    _txtPower.text = pwrTimeLeft;
                else _txtPower.text = string.Empty;
                string pwrCurrLoad = string.Empty;
                if (_co.mapInfo.TryGetValue("PowerCurrentLoad", out pwrCurrLoad))
                    _txtPowerRate.text = pwrCurrLoad;
                else _txtPowerRate.text = string.Empty;
                _sliderStatPower.value = Mathf.Clamp01((float)percPwr);
                bool pwrActive = !string.IsNullOrEmpty(pwrCurrLoad) && pwrCurrLoad[0] == '+';
                _arrowContainer.gameObject.SetActive(pwrActive);
                _arrowContainer.anchoredPosition = (!(_sliderStatPower.value < 0.3f)) ?
                    new Vector2(0f, _arrowContainer.anchoredPosition.y) :
                    new Vector2(14f, _arrowContainer.anchoredPosition.y);
            } else if (_poweredCO != null) {
                _sliderStatPower.value = Mathf.Clamp01((float)_poweredCO.PowerStoredPercent);
            }
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* StatusbarModule.OnUpdateUI
protected override void OnUpdateUI()
{
	if (_co == null)
	{
		return;
	}
	UpdateDamageBar();
	if (_co.HasCond("IsPowerObservable"))
	{
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		num2 += _co.GetCondAmount("StatPowerMax") * _co.GetDamageState();
		num += _co.GetCondAmount("StatPower");
		if (_co.GetComponent<Container>() != null)
		{
			List<CondOwner> cOs = _co.GetComponent<Container>().GetCOs(bAllowLocked: true);
			if (cOs != null && cOs.Count > 0)
			{
				foreach (CondOwner item in cOs)
				{
					if (item != null)
					{
						num2 += item.GetCondAmount("StatPowerMax") * item.GetDamageState();
						num += item.GetCondAmount("StatPower");
					}
				}
			}
		}
		if (num2 != 0.0)
		{
			num3 = num / num2;
		}
		if (num3 > 1.0)
		{
			num3 = 1.0;
		}
		string value = "-";
		if (_co.mapInfo.TryGetValue("PowerRemainingTime", out value))
		{
			_txtPower.text = value;
		}
		else
		{
			_txtPower.text = string.Empty;
		}
		string value2 = string.Empty;
		if (_co.mapInfo.TryGetValue("PowerCurrentLoad", out value2))
		{
			_txtPowerRate.text = value2;
		}
		else
		{
			_txtPowerRate.text = string.Empty;
		}
		_sliderStatPower.value = Mathf.Clamp01((float)num3);
		bool active = !string.IsNullOrEmpty(value2) && value2[0] == '+';
		_arrowContainer.gameObject.SetActive(active);
		_arrowContainer.anchoredPosition = ((!(_sliderStatPower.value < 0.3f)) ? new Vector2(0f, _arrowContainer.anchoredPosition.y) : new Vector2(14f, _arrowContainer.anchoredPosition.y));
	}
	else if (_poweredCO != null)
	{
		_sliderStatPower.value = Mathf.Clamp01((float)_poweredCO.PowerStoredPercent);
	}
}
*/