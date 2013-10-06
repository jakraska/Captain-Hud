function CHEditAnnounce::onWake(%this)
{
    CHPeriod.Setvalue($CHPrefs::SPAM_PROTECTION_PERIOD);
    CHThreshold.setvalue($CHPrefs::SPAM_MESSAGE_THRESHOLD);
    CHMaxLen.setvalue($CHPrefs::MaxMessageLen);
}
function CHEditAnnounce::ResetDefaults(%this)
{
    CHPeriod.Setvalue(10000);
    CHThreshold.setvalue(4);
    CHMaxLen.setvalue(120);

}
function CHEditAnnounce::OK(%this)
{
    $CHPrefs::SPAM_PROTECTION_PERIOD =  CHPeriod.GetValue();
    $CHPrefs::SPAM_MESSAGE_THRESHOLD = CHThreshold.getvalue();
    $CHPrefs::MaxMessageLen = CHMaxLen.getvalue();
    CHSavePrefs();
    canvas.popdialog(CHEditAnnounce);
}
