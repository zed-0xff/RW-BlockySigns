using System.Collections.Generic;

#pragma warning disable CS0414

public class _IntVec3 {
}

public class _Map {
    public static _Map instance = new _Map();

    private List<_Thing> loadedFullThings;
    public List<_Thing> LoadedFullThings { get { return loadedFullThings; }}

    public _Map(){
        loadedFullThings = new List<_Thing>();
        loadedFullThings.Add(_Thing.instance);
    }
}

public class _Thing {
    public static _Thing instance = new _Thing();
}

public class _Building {
    public static _Building instance = new _Building();

    public _IntVec3 Position { get { return new _IntVec3(); } }
}

public class _Room {
    public static _Room instance = new _Room();

    public int Temperature { get { return 42; }}
}

public class _Gizmo_RoomStats {
     public static _Room GetRoomToShowStatsFor(_Building building){
         return _Room.instance;
     }
}

public static class _Find {
    public static _Map CurrentMap { get { return _Map.instance; }}
    public static _Map anotherMap = null;
}

public class _Comp {
    public static _Comp instance = new _Comp();

    float mf(){
        return 1.23f;
    }

    int mi(){
        return 111;
    }

    int mi2(int x){
        return 111*x;
    }

    static int smi() {
        return 222;
    }

    int fi = 333;
    float ff = 4.56f;
    static int sfi = 444;

    public _Building parent { get { return _Building.instance; } }
}

