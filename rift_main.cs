using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using NewWidget.Core;
using NewWidget.Core.Native;
using NewWidget.Core.Scripting;

public class Script: ScriptBase {

    bool first_time = true;
    bool shield_global = false;

    int zone_where = -1;
    int id_buff = 4703; // Id of buff to perform check upon
    int DEFAULT_FARM_CHECK = 500;
    int DEFAULT_CHECK_RANGE = 300;

    string cold = "Sega";   // Party Member 1
    string troyki = "Mendes"; // Party Member 2

    Vector[] farm_room = new Vector[]{                     
        new Vector(-114802, -184693, -6743), // 31917
        new Vector(-113817, -186043, -6743)  // 31912
    };

    Vector[] other_room = new Vector[]{
        new Vector(-114942, -181102, -6729), // 1st spawn
        new Vector(-114571, -180939, -6729), // 2nd spawn
        new Vector(-115707, -184394, -6743), // 31910
        new Vector(-115701, -185251, -6743), // 31911
        new Vector(-113815, -186916, -6743), // 31913
        new Vector(-114784, -183009, -6743), // 31914
        new Vector(-115691, -186490, -6743), // 31916
        new Vector(-114790, -186386, -6743), // 31918
        new Vector(-113855, -184711, -6743),  // 31915
        new Vector(-114794, -181967, -6727)  // NPC
    };

    Vector[] case_0_runpath_npc = new Vector[]{
        new Vector(-114952, -181256, -6736),
        new Vector(-114792, -182104, -6736)
    };

    Vector[] case_1_runpath_npc = new Vector[]{
        new Vector(-114600, -181096, -6736),
        new Vector(-114792, -182104, -6736)
    };
 
    void my_status()
    {
        if(Me.IsDead)
        {
            Wait(3000);
            Client.Pve.Enabled = false;
            Client.RestartPoint(RestartPointTypes.Town);
            Wait(5000);
        }
    }

    public override void OnStart() {
        base.OnStart();
        CreateTimer(1000, temp_test);
    }

    void check_buff_party(){
        Wait(2000);
        var bishop = PartyMembers.FirstOrDefault(p => p.Name == cold);
        var spoiler = PartyMembers.FirstOrDefault(p => p.Name == troyki);
        if(bishop.BuffEndtime(id_buff) == 0 || spoiler.BuffEndtime(id_buff) == 0 || Me.BuffEndtime(id_buff) == 0)
            shield_global = true;
        else
            shield_global = false;
    }

    void rebuff(){
        Wait(5000);
        move_to(new Vector(-114872, -180216, -6736));
        npc_dialog(51000);
        Client.DialogSelect(2);
        Wait(500);
        Client.DialogSelect(7);
        Wait(500);
        Client.DialogSelect(1);
        Wait(500);
        Client.DialogSelect(6);
        Wait(500);
        Client.DialogSelect(8);
        Wait(500);
        Client.DialogSelect(7);
        Wait(500);
        Client.DialogSelect(12); // Buff ID 4703
    }

    void temp_test(){
        check_buff_party();
        my_status();
        if(right_room()){
            Console.WriteLine("right_room() Farm");
            if(!Client.Pve.Enabled)
                Client.Pve.Enabled = true;
            switch(zone_where){
                case 0:
                    Client.LoadZone("rift_17");
                    break;
                case 1:
                    Client.LoadZone("rift_12");
                    break;
                default:
                    break;

            }
            return;
        }else{
            switch(where_i()){
                case 0:
                    if(shield_global && Me.BuffEndtime(id_buff) == 0){
                        Console.WriteLine("Going for rebuff");
                        rebuff();
                        break;
                    }else if(shield_global){
                        Console.WriteLine("Waiting for Party Members' rebuff");
                        break;
                    }else{
                        Console.WriteLine("Spawn Point 0");
                        first_time = true;
                        if(Client.Pve.Enabled){
                            Wait(2000);
                            Client.Pve.Enabled = false;
                        }
                        foreach(Vector i in case_0_runpath_npc)
                            move_to(i);
                        npc_dialog(31493);
                        Client.DialogSelect(0);
                        break;
                    }
                case 1:
                    if(shield_global && Me.BuffEndtime(id_buff) == 0){
                        Console.WriteLine("Going for rebuff");
                        rebuff();
                        break;
                    }else if(shield_global){
                        Console.WriteLine("Waiting for Party Members' rebuff");
                        break;
                    }else{
                        Console.WriteLine("Spawn Point 1");
                        first_time = true;
                        if(Client.Pve.Enabled){
                            Wait(2000);
                            Client.Pve.Enabled = false;
                        }
                        foreach(Vector i in case_1_runpath_npc)
                            move_to(i);
                        npc_dialog(31493);
                        Client.DialogSelect(0);
                        break;
                    }
                case 2:
                    teleport_between(31910);
                    break;
                case 3:
                    teleport_between(31911);
                    break;
                case 4:
                    teleport_between(31913);
                    break;
                case 5:
                    teleport_between(31914);
                    break;
                case 6:
                    teleport_between(31916);
                    break;
                case 7:
                    /* Special Case */
                    Console.WriteLine("RB Room");
                    Client.Pve.Enabled = false;
                    npc_dialog(31918);
                    Client.DialogSelect(0);
                    break;
                case 8:
                    teleport_between(31915);
                    break;
                case 9:
                    npc_dialog(31493);
                    Client.DialogSelect(0);
                    break;
                default:
                    /* Lost */
                    Console.WriteLine("LOST");
                    if(Client.Pve.Enabled){
                        Wait(2000);
                        Client.Pve.Enabled = false;
                    }
                    move_to(other_room[0]);
                    break;
            }
        }
    }

    bool right_room(){
        for(int i = 0; i < 2; i++){
            if(within_range(farm_room[i], DEFAULT_FARM_CHECK)){
                zone_where = i;
                return true;
            }
        }
            return false;
    }

    bool within_range(Vector temp, int range){
        return Me.Location.DistanceTo(temp) <= range;
    }

    int where_i(){
        for(int i = 0; i < 10; i++)
            if(within_range(other_room[i], 300)){
                return i;
            }
        return -1;
    }

    void teleport_between(int npc_id){
        if(Client.Pve.Enabled)
            Client.Pve.Enabled = false;
        npc_dialog(npc_id);
        Client.DialogSelect(first_time ? 0 : 1);
        first_time = !first_time;
    }
    
    void move_to(Vector temp){
        Client.MoveToLocation(temp);
    }

    void npc_dialog(int a){
        var NpcTarget = Npcs.FirstOrDefault(n => n.Id == a);
        if(NpcTarget != null)
        {
            Client.SetTarget(NpcTarget);
            if(Me.Target == NpcTarget)
            {
                Client.DialogOpen();
                Wait(1000);
            }
        }
    }
}