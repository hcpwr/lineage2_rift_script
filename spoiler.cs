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
    int id_buff = 4703; // Id of buff to perfome check upon
    int DEFAULT_FARM_CHECK = 500;
    int DEFAULT_CHECK_RANGE = 300;
    
    string cold = "Sega";   // Party Member 1
    string troyki = "Mendez"; // Party Member 2

    Vector[] farm_room = new Vector[]{                     
        new Vector(-114802, -184693, -6743), // 31917
        new Vector(-113817, -186043, -6743) // 31912
    };

    Vector[] other_room = new Vector[]{
        new Vector(-114942, -181102, -6729), // 1st spawn
        new Vector(-114571, -180939, -6729), // 2nd spawn
        new Vector(-115707, -184394, - 6743), // 31910
        new Vector(-115701, -185251, -6743), // 31911
        new Vector(-113815,-186916, -6743), // 31913
        new Vector(-114784, -183009, -6743), // 31914
        new Vector(-115691, -186490, -6743), // 31916
        new Vector(-114790, -186386, -6743), // 31918
        new Vector(-113855, -184711, -6743) // 31915

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

    void rebuff(){
        Wait(5000);
        move_to(other_room[0]);
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
        my_status();
        if(right_room()){
            Console.WriteLine("right_room() Farm GNOM");
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
                    if(Me.BuffEndtime(id_buff) == 0){
                        Console.WriteLine("Going for rebuff GNOM");
                        rebuff();
                        break;
                    }else{
                        Console.WriteLine("Spawn Point 0 GNOM");
                        first_time = true;
                        if(Client.Pve.Enabled){
                            Wait(2000);
                            Client.Pve.Enabled = false;
                        }
                        break;
                    }
                case 1:
                    if(Me.BuffEndtime(id_buff) == 0){
                        Console.WriteLine("Going for rebuff GNOM");
                        rebuff();
                        break;
                    }else{
                        Console.WriteLine("Spawn Point 1 GNOM");
                        first_time = true;
                        if(Client.Pve.Enabled){
                            Wait(2000);
                            Client.Pve.Enabled = false;
                        }
                        break;
                    }
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    /* Special Case */
                    break;
                case 8:
                    break;
                default:
                    /* Lost */
                    Console.WriteLine("GNOM LOST");
                    if(Me.BuffEndtime(id_buff) == 0){
                        if(Client.Pve.Enabled){
                            Wait(2000);
                            Client.Pve.Enabled = false;
                        }
                        Console.WriteLine("Going for rebuff GNOM");
                        rebuff();
                        break;
                    }
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
        for(int i = 0; i < 9; i++)
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