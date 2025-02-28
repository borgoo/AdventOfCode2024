﻿

namespace AdventOfCode2024.Day23
{
    internal class Day23 : Day
    {

        private static readonly bool _debugActive = false;
        private const char SPECIAL_CHAR = 't';

        protected override object SolveA(string input)
        {
            var (graph, specialComputers) = HandleInput(input, SPECIAL_CHAR);

            HashSet<string> seen = [];

            int res = 0;
            foreach (string node in graph.Keys) { 
            
                res+=NavigateGraph( graph, specialComputers, seen, node);

            }

            return res;

        }

        protected override object SolveB(string input)
        {
            throw new NotImplementedException();
        }
        private static (Dictionary<string, HashSet<string>> graph, HashSet<string> specialComputers) HandleInput(string input, char specialChar)
        {
            Dictionary<string, HashSet<string>> graph = [];
            HashSet<string> specialComputers = [];

            string[] connections = input.Split("\r\n");
            foreach (string connection in connections) {
                string[] fromTo = connection.Split('-');
                string from = fromTo[0];
               if(from[0] == specialChar) specialComputers.Add(from);
                string to = fromTo[1];
               if(to[0] == specialChar) specialComputers.Add(to);

                UpdateGraph(graph, from, to);
                UpdateGraph(graph, to, from);

            }

            return (graph, specialComputers);
        }

        private static void UpdateGraph(Dictionary<string, HashSet<string> > graph, string from, string to) {

            if (!graph.ContainsKey(from)) {
                graph.Add(from, []);        
            }
            graph[from].Add(to);

        }

        private static int NavigateGraph(
           Dictionary<string, HashSet<string>> graph,
           HashSet<string> specialComputers,
           HashSet<string> seen,
           string currNode,
           int currDepth = 1,
           string? father = null,
           string? grandFather = null
       )
        {

            int ans = 0;

            if (currDepth == 3)
            {

                if (father is null || grandFather is null) throw new NullReferenceException();

                if (
                    !graph[currNode].Contains(father) || !graph[currNode].Contains(grandFather) ||
                    !graph[father].Contains(currNode) || !graph[father].Contains(grandFather) ||
                    !graph[grandFather].Contains(currNode) || !graph[grandFather].Contains(father) 
                ) return 0;



                string[] nodes = [currNode, father, grandFather];
                Array.Sort(nodes);
                string hashSeen = string.Join(",", nodes);
                if (seen.Contains(hashSeen)) return 0;
                seen.Add(hashSeen);

                if (specialComputers.Contains(currNode) || specialComputers.Contains(father) || specialComputers.Contains(grandFather))
                {
                    if (_debugActive) Console.WriteLine(hashSeen);

                    return 1;
                }

                return 0;

            }

            foreach (string newNode in graph[currNode])
            {

                if (newNode == father || newNode == grandFather) continue;

                if (father is null)
                {
                    ans += NavigateGraph(graph, specialComputers, seen, newNode, currDepth + 1, currNode, null);
                }
                else if (grandFather is null)
                {

                    ans += NavigateGraph(graph, specialComputers, seen, newNode, currDepth + 1, currNode, father);
                }
                else
                {
                    throw new Exception(KEEP_IT_FUN);
                }

            }

            return ans;


        }



        const string KEEP_IT_FUN = @"

They delved too greedily and too deep, and disturbed that from which they fled. You know what they awoke in the darkness of Khazad-dûm: shadow and flame.

                      .     @$* @$3
                             '$Nueeed$$ed$$eeec$$
          ,            4$Lze@*$C2$b* ed(he*rb$CC$*$bc@$r
    /@ |~~            .e$$""W$$B$B$**  ^$  e""""##d?$Bd$$$Nc. ..      @\/~\
    \==|         4$kd*Cr$6F#""`  **   .*==      # '""**F#$I$b$*       |   I
       |         d$5N@$$""   ....eu$$$$$$N$*$zbeuu     #$d$$$$b.     / @/
      @/     . z$Ted*""$P zue$*9d$$$@#       W$e@B$$L.    ""#@$E$b@N
            #d$Id*P#  'Nd$$B$**""       .*,     ""#*N$$b$c   $$$*$$c
           .d#+C6J   @@$B$*""          -***-        ""#$$$$c   *$$$#$u
        ..u$l4@""^""zJ$7W*""              '*`            ^*$@$$$r ""$$E$@B>
        *@$l$P""+Rd$$N#""          *     /|\     *        '""$$$c.. ?E$*b
        z$ ""*.  .Jz$""           ***   / | \   ***         '*@N$b   d**N
      .z$JBR^bs@$$#          *   *   /  |  \   *  *         ""$l*9N ""bN$Nee
     4$$.C*   dB@""          ***    _/  /^\  \_   ***         '$$$z> 3$b$$#
      $""$e$  @*$""        *   *     \\^|   |^//    *   *        $$$u.^*$N$c
     JPd$%  @@d""        ***        ***********       ***       '$Ni$  $EP$
   :e$""*$  :et$          *         ***********        *         ^$$E  4$N$be
   ')$ud""  @6$                                                   9$$   $*@$""
    @F*$   *4P                       ./                          '$m#   .$$.
 u*""""""""""""""""""""""""h                     ##=====                    e#""""""""""""""""""""#
 E +e       ue. N                 ___##_______                 4F e=c     z*c
 #e$@e.. ..z6+6d""                #*************/               ^*cBe$u.  .$$@
    $ ^"""""""" 4F""  ze=eu              ********              z***hc ^""$ """"*"""" $
    $       ^F :*    3r                                  @""  e ""b  $       $
  .e$        N $  'be$L...                            ...?be@F  $F $       9F
 4"" $        $ $.  zm$*****h.                      ue""""""""*h6   J$"" $       4%
 $  $        $ $$u5e"" .     ""k                    d""       #$bu$F  $       4F
 ""N $        $ ^d%P  dF      $  .            .e   $     -c  ""N$F  .$       4F
  #$$        $  $4*. ""N.    zP  3r ..    ..  $c   *u     $  u$K$  4F       4L
   ^N$e.     3  F$k*. ""*C$$$# .z$"" '$    4L  ""$c. '#$eeedF  $$$9r JF       J$
    $'""$$eu. 4  F3""K$ .e=*CB$$$$L .e$    '$bc.u$***hd6C""""  4kF$4F $F     u@$F
    $   '""*$*@u N'L$B*""z*""""     ""$F"" 4k 4c '7$""      ""*$eu 4'L$J"" $   .e$*""4F
    $      '""hC*$ ""$#.P""          $me$""  #$*$       .  ^*INJL$""$  $e$$*#   4F
    $         $b""h "".F     $""     ^F        $       9r   #L#$FJEd#C@""      4L
   .$         $Jb   J""..  4b      uF        *k      J%    #c^ $"" d$        4L
  :""$         $k9   $ $%4c $Bme.ze$         '*$+eee@*$""  :r$    @L$        4$
  $ $         $$Jr  $d"" '$r ""*==*""            ""#**"""" $r  4$3r  db$F        4F
  $c$         $'*F  $""   '$            /\            $    *(L  $$$F         k
  #i*e.       $ 4>  $  ue $         \`.||.'/         'L c  $$ .L$d         .$
   ""b.""*e.    4 4   $  $%db=eL     `.<\||/>.'      e*+$/$r  $ '$""$       .d$$
    $^#+cC*mu 4r4   4r:6@F  $$    -----++-----    <$. ""N?N  F  $ $    ud$$* $
    $    ""*eJ""@L4   4k*3Ic.*""      .'`.      #*5.J$$..F  $ $ ue#2*""   $
    $       ""N.""@r  4Fd"" '$r        /.'||`.\        4$ '""N*d""  9.$#Ce*""     $
    $         ""e^""  'd"" uz$%           \/           '$czr""k#""  4Pu@""        $

";


    }
}
