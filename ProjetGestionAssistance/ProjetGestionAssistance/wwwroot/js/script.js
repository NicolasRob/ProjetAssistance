// Fonction créé par : Francis Paré 17-11-2017
// Elle est appelée lorsque l'utilisateur sélectionne un autre département
// Elle change les données du select équipe, pour  qu'il affiche
// les équipes qui font parti du département sélectionner
function changerEquipeParDepartement() {
    // On va chercher le Id du département selectionner 
    var id = $("#DepartementId option:selected").val();
    // on va chercher la liste de tout les équipes faisant partie de ce département
    $.ajax({
        type: "Post",
        url: "/Billet/ListeEquipeParDepartementId/" + id,
        datatype: "json",
        success: function (Equipe) {
            // On créé un nouveau select avec les nouvelles équipes
            var nouveauSelect = "<select name =\"EquipeId\" id=\"EquipeId\" class=\"form-control\"  onchange=\"changerCompteParEquipe()\">" +
                "<option selected= \"selected\" value= \"NULL\"> Aucune &eacutequipe </option>";
            for (i = 0; i < Object.keys(Equipe).length; i++) {
                nouveauSelect += " <option value=\"" + Equipe[i].id + "\">" + Equipe[i].nom + "</option> ";
            }
            nouveauSelect += "</select >";
            // On remplace ce qui est dans le select #EquipeId par le nouveau select
            $("#EquipeId").replaceWith(nouveauSelect);
            changerCompteParEquipe();
        }

    })

}

function changerCompteParEquipe() {

    var idEquipe = $("#EquipeId option:selected").val();
    $.ajax({

        type: "Post",
        url: "/Billet/ListeCompteParEquipeId/" + idEquipe,
        datatype: "json",
        success: function (Compte) {
            // On créé un nouveau select avec les nouvelles équipes

            var nouveauSelect = "<select name =\"CompteId\" id=\"CompteId\" class=\"form-control\" >";

            for (i = 0; i < Object.keys(Compte).length; i++) {
                nouveauSelect += " <option value=\"" + Compte[i].compteID + "\">" + Compte[i].description + "</option> ";
            }
            nouveauSelect += "</select >";
            // On remplace ce qui est dans le select #EquipeId par le nouveau select
            $("#CompteId").replaceWith(nouveauSelect);

        }

    })
}

function changerEquipeParDepartement2() {
    // On va chercher le Id du département selectionner 
    var id = $("#DepartementId option:selected").val();
    // on va chercher la liste de tout les équipes faisant partie de ce département
    $.ajax({
        type: "Post",
        url: "/Compte/ListeEquipeParDepartementId/" + id,
        datatype: "json",
        success: function (Equipe) {
            // On créé un nouveau select avec les nouvelles équipes
            var nouveauSelect = "<select name =\"EquipeId\" id=\"EquipeId\" class=\"form-control\"  onchange=\"changerCompteParEquipe2()\">";
            for (i = 0; i < Object.keys(Equipe).length; i++) {
                nouveauSelect += " <option value=\"" + Equipe[i].id + "\">" + Equipe[i].nom + "</option> ";
            }
            nouveauSelect += "</select >";
            // On remplace ce qui est dans le select #EquipeId par le nouveau select
            $("#EquipeId").replaceWith(nouveauSelect);
            changerCompteParEquipe();
        }

    })
}
