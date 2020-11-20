export default {
  productName: 'Jibberwock Allert',
  shortProductName: 'Allert',
  auth: {
    logIn: 'Log In',
    logOut: 'Log Out',
    signUp: 'Sign Up'
  },
  validationErrorMessages: {
    unableToLoadAvailableProductList: 'We can\'t download the list of available products right now. You can still create a tenant, but will need to enable the Allert functionality later.',
    invalidEmailAddress: 'Please provide a valid email address.',
    noName: 'Please provide a name.',
    nameTooLong: 'This name is too long. Please provide a shorter name.',
    noEmailAddress: 'Please provide an email address.',
    emailAddressTooLong: 'This email address is too long. Please provide a shorter email address.',
    noTier: 'Please select a product tier.',
    noIdentityProvider: 'Please select a sign-in provider.',
    identityProviderTooLong: 'This sign-in provider name is too long. Please provide a shorter sign-in provider.'
  },
  dropdownValues: {
    identityProvider: [
      { id: 'google.com', label: 'Google', icon: 'mdi-google' },
      { id: 'live.com', label: 'Microsoft', icon: 'mdi-microsoft' },
      { id: 'github.com', label: 'GitHub', icon: 'mdi-github' },
      { id: 'idp', label: '(new account)', icon: 'mdi-account' }
    ]
  },
  actions: {
  },
  dialogs: {
  },
  forms: {
    createTenant: {
      steps: {
        tenantDetails: 'Tenant Details',
        invitations: 'Invitations',
        readyToCreate: 'Create'
      },
      creationMessages: {
        summary: 'The "{tenant}" tenant will now be created.',
        somePaidPlans: 'When you click Sign Up, you will be redirected to a payment page. Once you complete this payment, you will be redirected to the tenant\'s home page.',
        noPaidPlans: 'When you click Sign Up, you will be redirected to the tenant\'s home page.'
      }
    },
    fields: {
      tenant: {
        name: 'Tenant name',
        useOwnContactDetails: 'Use my contact details',
        contactName: 'Contact name',
        contactEmailAddress: 'Contact email address',
        selectedProducts: 'Selected Products',
        selectedProductTier: 'Tier'
      },
      invitation: {
        emailAddress: 'Email Address',
        identityProvider: 'Sign-in Provider'
      }
    },
    buttons: {
      previous: 'Previous',
      next: 'Next',
      add: 'Add'
    }
  },
  notificationList: {
    sections: {
      invitations: {
        title: 'Invitations'
      },
      notifications: {
        title: 'Notifications',
        emptyList: 'You have no notifications.',
        priorityFormat: '{label} priority'
      }
    },
    notificationTypes: [
      { id: 1, label: 'Alert', alertType: 'info' },
      { id: 2, label: 'Information', alertType: 'info' },
      { id: 3, label: 'Error', alertType: 'error' },
      { id: 4, label: 'Warning', alertType: 'warning' }
    ],
    notificationPriorities: [
      { id: 1, label: 'Low' },
      { id: 2, label: 'Normal' },
      { id: 3, label: 'High' }
    ]
  },
  pages: {
    homepage: {
      title: 'Homepage',
      unauthenticated: {
        description: 'Cloud Alerting Engine'
      }
    },
    about: {
      title: 'About',
      buttons: {
        roadmap: 'Roadmap'
      }
    },
    roadmap: {
      title: 'Roadmap'
    },
    signUp: {
      title: 'Sign Up'
    }
  }
}
