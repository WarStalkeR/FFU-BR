#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002
#pragma warning disable IDE0051

using System;
using System.Collections.Generic;
using System.Linq;
using FFU_Beyond_Reach;
using Ostranauts.UI.MegaToolTip;
using UnityEngine;
using UnityEngine.EventSystems;
using Vectrosity;

public class patch_CrewSim : CrewSim {
    private extern void orig_MouseHandler();
    private void MouseHandler() {
        if (FFU_BR_Defs.EnableCodeFixes) new_MouseHandler();
        else orig_MouseHandler();
    }
    private void new_MouseHandler() {
        List<CondOwner> list = FindCOsAtMousePosition(null, bInteractive: false, bAllowLocked: true);
        bool flag = false;
        CondOwner selected = GUIMegaToolTip.Selected;
        if (selected != null && list.Remove(selected)) {
            list.Insert(0, selected);
        }
        for (int i = 0; i < list.Count; i++) {
            if (bRaiseUI) {
                break;
            }
            if (CanvasManager.IsCanvasQuitShowing()) {
                break;
            }
            if (list[i].IsHumanOrRobot) {
                tooltip.SetTooltipCrew(list[i], GUITooltip.TooltipWindow.Crew);
                flag = true;
                break;
            }
            if (workManager.COIDHasTasks(list[i].strID)) {
                tooltip.SetTooltipMulti(list, GUITooltip.TooltipWindow.Task);
                flag = true;
                break;
            }
        }
        if (!flag && tooltip.window != GUITooltip.TooltipWindow.QAB && tooltip.window != GUITooltip.TooltipWindow.MTT) {
            tooltip.SetTooltip(null, GUITooltip.TooltipWindow.Hide);
        }
        vLastMouse = Input.mousePosition;
        bool flag2 = (bRaiseUI && CanvasManager.State == CanvasManager.GUIState.SHIPGUI) || GUIQuickBar.IsBeingDragged;
        bool flag3 = goSelPart != null || goPaintJob != null;
        if (flag3) {
            Canvas component = CanvasManager.goCanvasGUI.GetComponent<Canvas>();
            RectTransform rectTransform = component.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(component.transform as RectTransform, Input.mousePosition, component.worldCamera, out var localPoint);
            if (goSelPart != null) {
                Item component2 = goSelPart.GetComponent<Item>();
                rectRotate.localPosition = localPoint + new Vector2(0f, component2.nHeightInTiles * 16 * 2);
            }
            if (goPaintJob != null) {
                goPaintJob.transform.localPosition = localPoint;
            }
        }
        if (!flag2) {
            if (Input.mouseScrollDelta.y != 0f && !EventSystem.current.IsPointerOverGameObject() && IsMouseOverGameWindow()) {
                vPanVelocity.z -= 0.2f * Input.mouseScrollDelta.y;
            }
            if (GetMouseButtonDown(0)) {
                vDragStart = Input.mousePosition;
            }
            if (GetMouseButtonDown(2)) {
                vDragStart = Input.mousePosition;
            } else if (GetMouseButtonDown(1)) {
                if (goSelPart != null || goPaintJob != null || guiPDA.JobsActive) {
                    guiPDA.HideJobPaintUI();
                    return;
                }
            } else if (GetMouseButton(0)) {
                if (!EventSystem.current.IsPointerOverGameObject() && !GUIQuickBar.IsBeingDragged) {
                    if (bShipEdit && GUIActionKeySelector.commandEyedropper.Held) {
                        List<CondOwner> mouseOverCO = GetMouseOverCO(new string[1] { "Default" }, null);
                        if (mouseOverCO.Count > 0) {
                            SetPartCursor(mouseOverCO[0].strCODef);
                        } else {
                            SetPartCursor(null);
                        }
                    } else if (flag3) {
                        Item item = null;
                        if (goSelPart != null) {
                            item = goSelPart.GetComponent<Item>();
                        }
                        if (jiLast != null && jiLast.strName == "Cancel") {
                            RemoveTasksAtMousePosition();
                        } else if (jiLast != null && (jiLast.strName == "Uninstall" || jiLast.strName == "Scrap" || jiLast.strName == "Repair" || jiLast.strName == "Dismantle")) {
                            List<CondOwner> list2 = FindCOsAtMousePosition(null, bInteractive: false, bAllowLocked: false);
                            foreach (CondOwner item3 in list2) {
                                if (GUIPDA.ctJobFilter != null && !GUIPDA.ctJobFilter.Triggered(item3)) {
                                    continue;
                                }
                                List<string> jobActions = item3.GetJobActions(jiLast.strName);
                                foreach (string item4 in jobActions) {
                                    Interaction interaction = DataHandler.GetInteraction(item4);
                                    if (interaction != null && interaction.Triggered(GetSelectedCrew(), item3, bStats: false, bIgnoreItems: true)) {
                                        Task2 task = new Task2();
                                        task.strDuty = "Construct";
                                        task.strInteraction = item4;
                                        task.strTargetCOID = item3.strID;
                                        task.strName = jiLast.strName + "Job" + item3.strID;
                                        workManager.AddTask(task);
                                        break;
                                    }
                                }
                            }
                        } else if (jiLast != null && jiLast.strName == "Haul") {
                            List<CondOwner> list3 = FindCOsAtMousePosition(null, bInteractive: false, bAllowLocked: false);
                            CondTrigger condTrigger = DataHandler.GetCondTrigger("TIsValidHaulSource");
                            foreach (CondOwner item5 in list3) {
                                if (condTrigger.Triggered(item5)) {
                                    Task2 task2 = new Task2();
                                    task2.strDuty = "Haul";
                                    task2.strInteraction = "ACTHaulItem";
                                    task2.strTargetCOID = item5.strID;
                                    task2.strName = "HaulJob" + item5.strID;
                                    workManager.AddTask(task2);
                                }
                            }
                        } else if (chkFill.isOn) {
                            FloodFill();
                        } else if (bShipEditBG) {
                            if (shipCurrentLoaded.BGItemFits(item)) {
                                shipCurrentLoaded.BGItemAdd(item);
                                Debug.Log(string.Concat("Placing BG at: ", item.transform.position, "; local: ", item.transform.localPosition));
                            }
                            goSelPart.layer = LayerMask.NameToLayer("Default");
                            float z = goSelPart.transform.rotation.eulerAngles.z;
                            SetPartCursor(goSelPart.name);
                            item = goSelPart.GetComponent<Item>();
                            item.fLastRotation = z;
                        } else if (item.CheckFit(item.rend.bounds.center, shipCurrentLoaded, TileUtils.aSelPartTiles)) {
                            nLastClickIndex = 0;
                            vLastClick = default(Vector2);
                            goSelPart.layer = LayerMask.NameToLayer("Default");
                            if (iaItmInstall != null) {
                                InstallFinish();
                                if (bContinuePaintingJob) {
                                    StartPaintingJob(jiLast);
                                } else {
                                    CondOwner selectedCrew = GetSelectedCrew();
                                    int hourFromS = MathUtils.GetHourFromS(StarSystem.fEpoch);
                                    if (selectedCrew != null && selectedCrew.Company.GetShift(hourFromS, selectedCrew).nID != 2) {
                                        selectedCrew.LogMessage(selectedCrew.FriendlyName + DataHandler.GetString("SHIFT_WARN_NONWORK"), "Bad", selectedCrew.strID);
                                    }
                                }
                                bContinuePaintingJob = true;
                            } else if (iaItmInstall == null) {
                                bPoolShipUpdates = true;
                                Debug.Log("bPoolShipUpdates = true");
                                shipCurrentLoaded.AddCO(goSelPart.GetComponent<CondOwner>(), bTiles: true);
                                float z2 = goSelPart.transform.rotation.eulerAngles.z;
                                SetPartCursor(goSelPart.GetComponent<CondOwner>().strName);
                                item = goSelPart.GetComponent<Item>();
                                item.fLastRotation = z2;
                            }
                        } else if (iaItmInstall == null) {
                            List<CondOwner> mouseOverCO2 = GetMouseOverCO(new string[1] { "Default" }, null);
                            CondOwner component3 = item.GetComponent<CondOwner>();
                            foreach (CondOwner item6 in mouseOverCO2) {
                                if (item6.CanStackOnItem(component3) > 0 && component3 != item6.StackCO(component3)) {
                                    goSelPart.layer = LayerMask.NameToLayer("Default");
                                    float z3 = goSelPart.transform.rotation.eulerAngles.z;
                                    SetPartCursor(component3.strCODef);
                                    item = goSelPart.GetComponent<Item>();
                                    item.fLastRotation = z3;
                                    break;
                                }
                            }
                        }
                    } else if (inventoryGUI.activeWindows.Count == 0) {
                        float num = Mathf.Abs(Input.mousePosition.x - vDragStart.x);
                        float num2 = Mathf.Abs(Input.mousePosition.y - vDragStart.y);
                        if ((num2 > 16f || num > 16f) && lineSelectRect == null) {
                            lineSelectRect = new VectorLine("SelectionRect", new List<Vector2>(5), 1.5f, LineType.Continuous, Joins.Weld);
                            lineSelectRect.color = Color.white;
                            lineSelectRect.SetCanvas(CanvasManager.goCanvasGUI, worldPositionStays: false);
                        }
                    }
                }
            } else if (GetMouseButtonUp(0)) {
                bPoolShipUpdates = false;
                GameObject gameObject = null;
                if (contextMenuPool.IsRaised) {
                    LowerContextMenu();
                } else if (lineSelectRect != null) {
                    VectorLine.Destroy(ref lineSelectRect);
                    SetBracketTarget(null, bUpdateOnly: false);
                    Bounds viewportBounds = GetViewportBounds(camMain, vDragStart, Input.mousePosition);
                    if (TileUtils.bShowTiles || bShipEdit) {
                        SelectBounds(viewportBounds, TileUtils.bShowTiles);
                    }
                } else if (!EventSystem.current.IsPointerOverGameObject()) {
                    if (TileUtils.bShowTiles) {
                        gameObject = ClickSelectScenePart(new string[1] { "Tile Helpers" });
                        CondOwner condOwner = null;
                        if (gameObject != null) {
                            condOwner = gameObject.GetComponent<CondOwner>();
                            Tile component4 = gameObject.GetComponent<Tile>();
                            SelectCO(condOwner);
                            int value = shipCurrentLoaded.aTiles.IndexOf(component4);
                            if (!GUIActionKeySelector.commandZoneAlternate.Held) {
                                foreach (JsonZone value2 in shipCurrentLoaded.mapZones.Values) {
                                    if (value2.aTiles.Contains(value)) {
                                        int[] aTiles = value2.aTiles;
                                        foreach (int index in aTiles) {
                                            SelectCO(shipCurrentLoaded.aTiles[index].coProps);
                                        }
                                        break;
                                    }
                                }
                            }
                        } else {
                            SetBracketTarget(null, bUpdateOnly: false);
                        }
                        OnTileSelectionUpdated.Invoke(aSelected);
                    } else if (flag3) {
                        Item item2 = null;
                        if (goSelPart != null) {
                            item2 = goSelPart.GetComponent<Item>();
                        }
                        if (item2 != null && item2.CheckFit(item2.rend.bounds.center, shipCurrentLoaded, TileUtils.aSelPartTiles)) {
                            nLastClickIndex = 0;
                            vLastClick = default(Vector2);
                            goSelPart.layer = LayerMask.NameToLayer("Default");
                            if (iaItmInstall != null) {
                                InstallFinish();
                                if (bContinuePaintingJob) {
                                    StartPaintingJob(jiLast);
                                } else {
                                    CondOwner selectedCrew2 = GetSelectedCrew();
                                    int hourFromS2 = MathUtils.GetHourFromS(StarSystem.fEpoch);
                                    if (selectedCrew2 != null && selectedCrew2.Company.GetShift(hourFromS2, selectedCrew2).nID != 2) {
                                        selectedCrew2.LogMessage(selectedCrew2.FriendlyName + DataHandler.GetString("SHIFT_WARN_NONWORK"), "Bad", selectedCrew2.strID);
                                    }
                                }
                                bContinuePaintingJob = true;
                            }
                        }
                    } else if (!bJustClickedInput) {
                        if (coConnectMode != null) {
                            gameObject = ClickSelectScenePart(new string[1] { "Tile Helpers" });
                        } else if (!bShipEditBG) {
                            CondTrigger condTrigger2 = ctSelectFilter;
                            if (bShipEdit || bDebugShow) {
                                ctSelectFilter = null;
                            } else {
                                ctSelectFilter = DataHandler.GetCondTrigger("TCanBeSelected");
                            }
                            List<CondOwner> mouseOverCO3 = GetMouseOverCO(new string[1] { "Default" }, null);
                            Room room = null;
                            if (shipCurrentLoaded != null) {
                                room = shipCurrentLoaded.GetRoomAtWorldCoords1(vMouse, bAllowDocked: true);
                            }
                            if (room != null) {
                                mouseOverCO3.Remove(room.CO);
                                mouseOverCO3.Add(room.CO);
                            }
                            if (!bShipEdit && mouseOverCO3.Count > 0 && mouseOverCO3.IndexOf(GUIMegaToolTip.Selected) >= 0) {
                                mouseOverCO3.Clear();
                                OnRightClick.Invoke(mouseOverCO3);
                            }
                            gameObject = ClickSelectScenePart(new string[1] { "Default" });
                            if (gameObject == null && !bShipEdit && mouseOverCO3.Count > 0) {
                                Walk();
                            }
                            ctSelectFilter = condTrigger2;
                        }
                        if (coConnectMode != null) {
                            CondOwner condOwner2 = null;
                            if (gameObject != null) {
                                condOwner2 = gameObject.GetComponent<CondOwner>();
                                if (!ctSelectFilter.Triggered(condOwner2)) {
                                    condOwner2 = null;
                                }
                            }
                            igdConnectMode.SetInput(condOwner2);
                            if (coConnectLastCrew != null) {
                                SetBracketTarget(coConnectLastCrew.strID, bUpdateOnly: false, noAuto: true);
                                coConnectLastCrew = null;
                                coConnectMode = null;
                            } else {
                                SetBracketTarget(null, bUpdateOnly: false);
                            }
                            HideInputSelector();
                            if (GUIModal.Instance != null) GUIModal.Instance.Hide(); // FIX
                        } else if (gameObject != null) {
                            CondOwner component5 = gameObject.GetComponent<CondOwner>();
                            if (component5.strCODef.IndexOf("Closed") >= 0 || component5.strCODef.IndexOf("Open") >= 0) {
                                Ship ship = component5.ship;
                                CondOwner condOwner3 = null;
                                string strCODef = component5.strCODef;
                                strCODef = ((strCODef.IndexOf("Open") < 0) ? strCODef.Replace("Closed", "Open") : strCODef.Replace("Open", "Closed"));
                                condOwner3 = DataHandler.GetCondOwner(strCODef, component5.strID);
                                if (condOwner3 != null) {
                                    component5.ModeSwitch(condOwner3, component5.tf.position);
                                }
                            }
                        } else if (CanvasManager.State == CanvasManager.GUIState.SOCIAL && GUISocialCombat2.coUs == GetSelectedCrew() && GUISocialCombat2.coUs != GUISocialCombat2.coThem) {
                            if (GUISocialCombat2.coUs.bAlive) {
                                Interaction interaction2 = DataHandler.GetInteraction("SOCSnub");
                                interaction2.objUs = GUISocialCombat2.coUs;
                                interaction2.objThem = GUISocialCombat2.coThem;
                                interaction2.bManual = true;
                                GUISocialCombat2.coUs.AIIssueOrder(interaction2.objThem, interaction2, bPlayerOrdered: true, null);
                                Paused = false;
                            } else {
                                GUISocialCombat2.objInstance.EndSocialCombat();
                            }
                        }
                    }
                }
            } else if (bShipEdit && GetMouseButtonDown(1)) {
                if (goSelPart != null) {
                    SetPartCursor(null);
                    return;
                }
                if (TileUtils.bShowTiles) {
                    SetBracketTarget(null, bUpdateOnly: false);
                    return;
                }
                if (bShipEditBG) {
                    List<Item> mouseOverBG = GetMouseOverBG(new string[1] { "Default" });
                    if (mouseOverBG != null && mouseOverBG.Count > 0) {
                        shipCurrentLoaded.BGItemRemove(mouseOverBG[0]);
                    }
                } else {
                    nLastClickIndex = 0;
                    vLastClick = default(Vector2);
                    CondTrigger condTrigger3 = ctSelectFilter;
                    ctSelectFilter = new CondTrigger();
                    List<string> list4 = new List<string>();
                    if (condTrigger3 != null) {
                        list4.AddRange(condTrigger3.aForbids);
                        Array.Copy(condTrigger3.aReqs, ctSelectFilter.aReqs, condTrigger3.aReqs.Length);
                    }
                    list4.Add("IsRoom");
                    ctSelectFilter.aForbids = list4.ToArray();
                    GameObject gameObject2 = ClickSelectScenePart(new string[1] { "Default" });
                    if (gameObject2 != null) {
                        shipCurrentLoaded.RemoveCO(gameObject2.GetComponent<CondOwner>());
                        UnityEngine.Object.Destroy(gameObject2);
                    }
                    ctSelectFilter = condTrigger3;
                }
            } else if (GetMouseButtonUp(1)) {
                if (!bJustClickedInput) {
                    if (ZoneMenuOpen) {
                        return;
                    }
                    if (objInstance.coConnectMode != null) {
                        if (coConnectLastCrew != null) {
                            SetBracketTarget(coConnectLastCrew.strID, bUpdateOnly: false, noAuto: true);
                            coConnectLastCrew = null;
                            coConnectMode = null;
                        } else {
                            SetBracketTarget(null, bUpdateOnly: false);
                        }
                        HideInputSelector();
                        GUIModal.Instance.Hide();
                    } else if ((contextMenuPool.IsRaised && !bRaisedMenuThisFrame) || (double)RightMouseButtonDownTimer > 0.3) {
                        RightMouseButtonDownTimer = 0f;
                        LowerContextMenu();
                    } else if (!bRaiseUI && !inventoryGUI.ClickedInventory(Input.mousePosition)) {
                        RightMouseButtonDownTimer = 0f;
                        if (CanvasManager.IsOverUIElement(goCrewBar)) {
                            if (CanvasManager.IsOverUIElement(goCrewBarPortraitButton)) {
                                OnRightClick.Invoke(new List<CondOwner> { GetSelectedCrew() });
                            }
                        } else {
                            if (bShipEdit || bDebugShow) {
                                ctSelectFilter = null;
                            } else {
                                ctSelectFilter = DataHandler.GetCondTrigger("TCanBeSelectedMTT");
                            }
                            List<CondOwner> mouseOverCO4 = GetMouseOverCO(new string[2] { "Default", "LoS" }, ctSelectFilter);
                            Room room2 = null;
                            if (shipCurrentLoaded != null) {
                                room2 = shipCurrentLoaded.GetRoomAtWorldCoords1(vMouse, bAllowDocked: true);
                            }
                            if (room2 != null) {
                                mouseOverCO4.Remove(room2.CO);
                                mouseOverCO4.Add(room2.CO);
                            }
                            if (mouseOverCO4 != null && mouseOverCO4.Count > 0) {
                                OnRightClick.Invoke(mouseOverCO4);
                            }
                            ctSelectFilter = null;
                        }
                    }
                }
            } else if (GetMouseButton(2)) {
                float num3 = Input.mousePosition.x - vDragStart.x;
                float num4 = Input.mousePosition.y - vDragStart.y;
                float num5 = 1f;
                if (camMain != null) {
                    num5 = camMain.aspect;
                }
                delX += num3 / 15f * num5;
                delY += num4 / 15f;
            }
            if (goSelPart != null) {
                Item component6 = goSelPart.GetComponent<Item>();
                if (component6 != null) {
                    component6.SetToMousePosition(vMouse);
                    if (component6.jid.strName != "Cancel") {
                        component6.CheckFit(component6.rend.bounds.center, shipCurrentLoaded, TileUtils.aSelPartTiles);
                    }
                }
                CondOwner component7 = goSelPart.GetComponent<CondOwner>();
                Powered component8 = goSelPart.GetComponent<Powered>();
                if (component8 != null && !component7.HasCond("IsPowerInputIgnore") && component8.jsonPI.aInputPts != null && component8.jsonPI.aInputPts.Length > 0) {
                    for (int k = 0; k < component8.jsonPI.aInputPts.Length; k++) {
                        Vector3 position = component7.GetPos(component8.jsonPI.aInputPts[k]);
                        position.z = -8f;
                        TileUtils.GetPowerInputGridSprite(k).transform.position = position;
                        TileUtils.GetPowerInputGridSprite(k).SetActive(value: true);
                    }
                }
                Vector3 zero = Vector3.zero;
                if (component7.mapPoints.ContainsKey("PowerOutput")) {
                    zero = component7.GetPos("PowerOutput");
                    zero.z = -8f;
                    TileUtils.GetPowerOutputGridSprite().transform.position = zero;
                    TileUtils.GetPowerOutputGridSprite().SetActive(value: true);
                }
                zero = Vector3.zero;
                if (component7.mapPoints.ContainsKey("use")) {
                    zero = component7.mapPoints["use"];
                    if (zero.x != 0f || zero.y != 0f) {
                        zero = component7.GetPos("use");
                        zero.z = -8f;
                        TileUtils.GetUseGridSprite().transform.position = zero;
                        TileUtils.GetUseGridSprite().SetActive(value: true);
                    }
                }
                zero = Vector3.zero;
                if (component7.mapPoints.ContainsKey("ReactorPlug")) {
                    zero = component7.GetPos("ReactorPlug");
                    zero.z = -8f;
                    TileUtils.GetReactorGridSprite().transform.position = zero;
                    TileUtils.GetReactorGridSprite().SetActive(value: true);
                }
            }
        }
        bJustClickedInput = false;
    }
}