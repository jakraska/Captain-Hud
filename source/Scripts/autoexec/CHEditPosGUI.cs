function CHEditPos::onWake(%this)
{
   RefreshEditPosList();
}
function RefreshEditPosList()
{
   Pos_List.clear();
   for(%i = 0; %i < $CHPOS::Count; %i++)
   {
       Pos_List.addrow( %i, $CHPOS::Pos[%i]);
   }
   Pos_List.sort(0);

}
function CHEditPos::AddPos(%this)
{
   %pos = PositionsTxt.getvalue();
   if(%pos $= "")
      return;
   //make sure it isn't already on the list
   %found = false;
   for(%i = 0; %i < $CHPOS::Count; %i++)
   {
      if(%pos $= $CHPOS::Pos[%i])
      {
         %found = true;
         break;
      }
   }
   if(%found)
   {
      MessageBoxOK( "ERROR", %pos SPC "is already part of the list.");
      return;
   }
   //Ok we are good to go
   $CHPOS::Pos[$CHPOS::Count] = %pos;
   $CHPOS::Count++;
   PositionsTxt.SetValue("");
   RefreshEditPosList();
}
function CHEditPos::RemovePos(%this)
{
    %ID = Pos_List.GetselectedID();
    if(%ID == -1)
       return;
    for(%i = %ID; %i < $CHPOS::Count; %i++)
    {
        $CHPOS::Pos[%i] =  $CHPOS::Pos[%i + 1];
    }
    $CHPOS::Count--;
    RefreshEditPosList();
}

function CHEditPos::Done(%this)
{
   %file = "prefs/CaptainHud/Positions.cs";
   export( "$CHPOS::*", %file, false );
   CHEditMPL::RefreshPositions();
   CaptainHud::RefreshPositions();
   canvas.PopDialog(CHEditPos);

}
