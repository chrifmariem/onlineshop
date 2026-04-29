using onlineShop.Models;
using System.Text;

namespace onlineShop.Services
{
    public class ProductAssistantService : IProductAssistantService
    {
        public async Task<string> GetProductGuidanceAsync(Produit product, string userQuestion)
        {
            await Task.Delay(500);
            var lowerQuestion = userQuestion.ToLower();

            // Réponses amicales
            var friendly = GetFriendlyResponse(lowerQuestion);
            if (!string.IsNullOrEmpty(friendly)) return friendly;

            // Détecter le type de produit
            var productType = GetProductType(product.design);

            // Traiter les questions principales
            if (ContainsAny(lowerQuestion, "comment utiliser", "mode d'emploi", "appliquer", "étapes"))
                return GetUsageInstructions(product, productType);

            if (ContainsAny(lowerQuestion, "conserv", "stockage"))
                return GetStorageInstructions(product);

            if (ContainsAny(lowerQuestion, "précaution", "contre indication"))
                return GetPrecautions(productType);

            if (ContainsAny(lowerQuestion, "fréquence", "combien de fois"))
                return GetFrequency(productType);

            if (ContainsAny(lowerQuestion, "durée", "combien de temps"))
                return GetDuration(productType);

            if (ContainsAny(lowerQuestion, "composition", "ingrédient"))
                return GetIngredients(product.design);

            if (ContainsAny(lowerQuestion, "peau", "type de peau", "quel peau", "pour quelle peau"))
                return GetSkinTypeAdvice(product.design);

            return GetGeneralResponse(product.design);
        }

        // Méthodes auxiliaires simplifiées
        private string GetFriendlyResponse(string question)
        {
            if (ContainsAny(question, "bonjour", "salut", "hello"))
                return "Bonjour ! 😊 Comment puis-je vous aider ?";
            if (ContainsAny(question, "merci", "thanks"))
                return "Avec plaisir ! 😊";
            if (ContainsAny(question, "au revoir", "bye"))
                return "À bientôt !";
            return null;
        }

        private string GetProductType(string productName)
        {
            var name = productName.ToUpper();
            if (name.Contains("LAIT")) return "LAIT";
            if (name.Contains("GEL")) return "GEL";
            if (name.Contains("CREME") || name.Contains("BAUME")) return "CREME";
            if (name.Contains("HUILE")) return "HUILE";
            if (name.Contains("MASQUE") || name.Contains("PEEL")) return "MASQUE";
            if (name.Contains("ONGLES")) return "ONGLES";
            if (name.Contains("PAUPIERE")) return "PAUPIERES";
            return "GENERAL";
        }

        private string GetUsageInstructions(Produit product, string type)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<div class='pharmacist-instructions'>");
            sb.AppendLine($"<h6><i class='fas fa-user-md'></i> Mode d'emploi - {product.design}</h6>");

            sb.AppendLine("<p><strong>Étapes générales :</strong></p>");
            sb.AppendLine("<ol>");
            sb.AppendLine("<li>Lavez-vous les mains</li>");
            sb.AppendLine("<li>Nettoyez et séchez la zone</li>");
            sb.AppendLine("<li>Appliquez une petite quantité</li>");
            sb.AppendLine("<li>Massez jusqu'à pénétration</li>");
            sb.AppendLine("</ol>");

            // Conseils spécifiques
            switch (type)
            {
                case "LAIT":
                    sb.AppendLine("<div class='alert alert-info'><i class='fas fa-info-circle'></i> Appliquer 1-2 fois/jour après la douche</div>");
                    break;
                case "GEL":
                    sb.AppendLine("<div class='alert alert-info'><i class='fas fa-info-circle'></i> Rincer abondamment après application</div>");
                    break;
                case "CREME":
                    sb.AppendLine("<div class='alert alert-info'><i class='fas fa-info-circle'></i> Appliquer sur peau propre et sèche</div>");
                    break;
                case "HUILE":
                    sb.AppendLine("<div class='alert alert-info'><i class='fas fa-info-circle'></i> Idéal pour peau sensible et démaquillage</div>");
                    break;
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        private string GetStorageInstructions(Produit product)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class='storage-info'>");
            sb.AppendLine("<h6><i class='fas fa-temperature-low'></i> Conservation</h6>");

            sb.AppendLine("<ul>");
            sb.AppendLine("<li>Température ambiante (15-25°C)</li>");
            sb.AppendLine("<li>À l'abri de la lumière directe</li>");
            sb.AppendLine("<li>Hors de portée des enfants</li>");
            sb.AppendLine("</ul>");

            if (product.dlc.HasValue)
            {
                sb.AppendLine($"<div class='alert alert-info'><i class='fas fa-calendar-alt'></i> DLC : {product.dlc.Value:dd/MM/yyyy}</div>");
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        private string GetPrecautions(string type)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class='precautions-info'>");
            sb.AppendLine("<h6><i class='fas fa-exclamation-triangle'></i> Précautions</h6>");

            sb.AppendLine("<ul>");
            sb.AppendLine("<li>Usage externe uniquement</li>");
            sb.AppendLine("<li>Ne pas avaler</li>");
            sb.AppendLine("<li>Éviter contact avec les yeux</li>");
            sb.AppendLine("<li>Test sur petite zone si peau sensible</li>");
            sb.AppendLine("</ul>");

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        private string GetFrequency(string type)
        {
            var frequency = type switch
            {
                "LAIT" or "CREME" => "1-2 fois par jour",
                "GEL" => "1-2 fois par jour (lavant)",
                "MASQUE" => "1 fois par semaine",
                "ONGLES" => "1 fois par jour",
                _ => "Selon les besoins"
            };

            return $"<div class='alert alert-info'><i class='fas fa-redo'></i> <strong>Fréquence recommandée :</strong> {frequency}</div>";
        }

        private string GetDuration(string type)
        {
            var duration = type switch
            {
                "ONGLES" => "3 mois minimum",
                "MASQUE" => "4-6 semaines pour résultat optimal",
                _ => "Utilisation continue recommandée"
            };

            return $"<div class='alert alert-info'><i class='fas fa-hourglass-half'></i> <strong>Durée :</strong> {duration}</div>";
        }

        private string GetIngredients(string productName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class='ingredients-info'>");
            sb.AppendLine("<h6><i class='fas fa-flask'></i> Composition</h6>");

            if (productName.Contains("XERIAL"))
            {
                sb.AppendLine("<p><strong>Actifs principaux XERIAL :</strong></p>");
                sb.AppendLine("<ul>");
                sb.AppendLine("<li>Urée (hydratation intense)</li>");
                sb.AppendLine("<li>Actifs kératolytiques</li>");
                sb.AppendLine("<li>Agent filmogène protecteur</li>");
                sb.AppendLine("</ul>");
            }
            else if (productName.Contains("TOPIALYSE"))
            {
                sb.AppendLine("<p><strong>Actifs principaux TOPIALYSE :</strong></p>");
                sb.AppendLine("<ul>");
                sb.AppendLine("<li>I-modulia®</li>");
                sb.AppendLine("<li>Acide hyaluronique</li>");
                sb.AppendLine("<li>Beurre de karité</li>");
                sb.AppendLine("</ul>");
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        private string GetSkinTypeAdvice(string productName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class='skintype-info'>");
            sb.AppendLine("<h6><i class='fas fa-user-circle'></i> Type de peau adapté</h6>");

            if (productName.Contains("XERIAL"))
            {
                sb.AppendLine("<p><strong>Ce produit est idéal pour :</strong></p>");
                sb.AppendLine("<ul>");
                sb.AppendLine("<li><strong>Peaux très sèches</strong> et rugueuses</li>");
                sb.AppendLine("<li><strong>Hyperkératose</strong> (épaississement cutané)</li>");
                sb.AppendLine("<li><strong>Callosités et durillons</strong></li>");
                sb.AppendLine("<li><strong>Fissures et crevasses</strong></li>");
                sb.AppendLine("<li>Peau avec <strong>psoriasis</strong> (sous contrôle médical)</li>");
                sb.AppendLine("</ul>");
            }
            else if (productName.Contains("TOPIALYSE"))
            {
                sb.AppendLine("<p><strong>Ce produit est idéal pour :</strong></p>");
                sb.AppendLine("<ul>");
                sb.AppendLine("<li><strong>Peaux sensibles</strong> et réactives</li>");
                sb.AppendLine("<li><strong>Peau atopique</strong> (eczéma)</li>");
                sb.AppendLine("<li><strong>Peau intolérante</strong></li>");
                sb.AppendLine("<li><strong>Sécheresse cutanée</strong></li>");
                sb.AppendLine("<li>Après <strong>traitements dermatologiques</strong></li>");
                sb.AppendLine("</ul>");
            }
            else
            {
                sb.AppendLine("<p><strong>Conseil général :</strong></p>");
                sb.AppendLine("<ul>");
                sb.AppendLine("<li>Convient à tous types de peau</li>");
                sb.AppendLine("<li>Testez sur petite zone si peau sensible</li>");
                sb.AppendLine("<li>Consultez un dermatologue pour un avis personnalisé</li>");
                sb.AppendLine("</ul>");
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        private string GetGeneralResponse(string productName)
        {
            return $@"
            <div class='pharmacist-response'>
                <h6><i class='fas fa-comment-medical'></i> Conseils</h6>
                <p>Pour des informations spécifiques sur <strong>{productName}</strong>, demandez-moi :</p>
                <ul>
                    <li>Comment utiliser ce produit ?</li>
                    <li>Pour quel type de peau ?</li>
                    <li>Comment le conserver ?</li>
                    <li>Quelles précautions respecter ?</li>
                </ul>
            </div>";
        }

        // Méthode utilitaire
        private bool ContainsAny(string text, params string[] patterns)
        {
            foreach (var pattern in patterns)
                if (text.Contains(pattern)) return true;
            return false;
        }
    }

   
}