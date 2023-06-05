#pragma warning disable CS0414

public class _IntVec3 {
}

public class _Map {
    public static _Map instance = new _Map();
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

public class _Comp {
    public static _Comp instance = new _Comp();

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
    static int sfi = 444;

    public _Building parent { get { return _Building.instance; } }
}

