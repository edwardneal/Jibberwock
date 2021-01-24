export default {
  productName: 'Jibberwock Allert',
  shortProductName: 'Allert',
  auth: {
    logIn: 'Sign Up / Log In',
    logOut: 'Log Out',
    signUp: 'Sign Up',
    editProfile: 'Edit Profile',
    resetPassword: 'Change Password'
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
    identityProviderTooLong: 'This sign-in provider name is too long. Please provide a shorter sign-in provider.',
    unableToLoadTenantList: 'We can\'t get the list of tenants right now. Please try again later.',
    unableToCreateTenant: 'We can\'t create a tenant right now. Please try again later.',
    unableToGetTenant: 'We can\'t get this tenant right now. Please try again later.',
    unableToGetSecurityGroup: 'We can\'t get this security group right now. Please try again later.',
    cannotDeleteWellKnownGroup: 'This group is one of this tenant\'s well-known groups, so it cannot be deleted.'
  },
  dropdownValues: {
    identityProvider: [
      { id: 'google.com', label: 'Google', icon: 'mdi-google' },
      { id: 'live.com', label: 'Microsoft', icon: 'mdi-microsoft' },
      { id: 'github.com', label: 'GitHub', icon: 'mdi-github' },
      { id: 'Azure Active Directory', label: '(new account)', icon: 'mdi-account' }
    ],
    securableResourceType: [
      { id: 1, label: 'Tenant', icon: 'mdi-account-group', allowAdd: true },
      { id: 2, label: 'API Key', icon: 'mdi-api', allowAdd: true },
      { id: 3, label: 'Product', icon: 'mdi-package-variant', allowAdd: false },
      { id: 4, label: 'Service', icon: 'mdi-file-cloud', allowAdd: false },
      { id: 5, label: 'Alert Definition', icon: 'mdi-cloud-alert', allowAdd: true },
      { id: 6, label: 'Alert Definition Group', icon: 'mdi-table-cog', allowAdd: true }
    ],
    permission: [
      { id: 1, label: 'Read', allowAdd: true },
      { id: 2, label: 'Change', allowAdd: true },
      { id: 3, label: 'Change Billing Contact', allowAdd: true },
      { id: 4, label: 'Change Subscription Billing', allowAdd: true },
      { id: 5, label: 'Delete', allowAdd: true },
      { id: 6, label: 'Invite', allowAdd: true },
      { id: 7, label: 'Read Logs', allowAdd: true },
      { id: 8, label: 'Create API Key', allowAdd: true },
      { id: 9, label: 'Create Product', allowAdd: false }
    ]
  },
  actions: {
    create: 'Create',
    delete: 'Delete',
    edit: 'Edit',
    update: 'Update',
    cancel: 'Cancel',
    invite: 'Invite'
  },
  dialogs: {
    updateTenantSecurityGroup: {
      title: 'Update Group',
      fields: {
        group: {
          name: 'Name',
          members: 'Members',
          accessControlEntries: 'Security'
        },
        member: {
          name: 'User Name',
          enabled: 'Enabled?'
        },
        accessControlEntry: {
          resourceName: 'Resource',
          permission: 'Permission'
        }
      }
    }
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
      },
      tooltips: {
        freeTier: 'This product tier is free to use.',
        paidTier: 'This product tier will require payment.'
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
      add: 'Create'
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
  tenantList: {
    sections: {
      invitations: {
        title: 'Invitations',
        buttons: {
          accept: 'Accept',
          reject: 'Reject'
        }
      },
      tenants: {
        title: 'Tenants',
        noTenants: 'Your account isn\'t a member of any tenants. Use the Create button to create one.',
        loading: 'Loading your account\'s list of tenants...',
        buttons: {
          subscriptions: 'Subscriptions',
          security: 'Security',
          allert: 'Allert Configuration',
          apiKeys: 'API Keys'
        }
      }
    }
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
    },
    tenant: {
      title: 'Tenant Details',
      tabs: {
        alertDefinitionGroups: 'Alert Definition Groups',
        alertDefinitions: 'Alert Definitions',
        activeAlerts: 'Active Alerts'
      }
    },
    tenant_security: {
      title: 'Tenant Security',
      tabs: {
        users: 'Users',
        invitations: 'Invitations',
        roles: 'Groups'
      },
      sections: {
        roles: {
          headings: {
            roleName: 'Group Name',
            wellKnownGroupName: 'Well-Known Group',
            members: 'Members',
            permissions: 'Permissions'
          },
          emptyLists: {
            members: 'Nobody has been added to this group yet.'
          }
        }
      }
    },
    tenant_subscriptions: {
      title: 'Tenant Subscriptions'
    },
    tenant_api_keys: {
      title: 'Tenant API Keys'
    }
  }
}
