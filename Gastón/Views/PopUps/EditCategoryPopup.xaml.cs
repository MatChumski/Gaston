using Gastón.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gastón.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCategoryPopup
    {
        public EditCategoryPopup()
        {
            InitializeComponent();
        }

        public EditCategoryPopup(ConfigViewModel.UserCategoriesInfo categoryInfo)
        {
            InitializeComponent();
            BindingContext = new EditCategoryViewModel(categoryInfo);
        }        
    }
}