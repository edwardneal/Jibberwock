export default {
  productName: 'Jibberwock Admin',
  auth: {
    logIn: 'Log In',
    logOut: 'Log Out'
  },
  sections: {
    users: {
      title: 'Users',
      items: {
        users: 'Users',
        tenants: 'Tenants'
      }
    },
    service: {
      title: 'Service',
      items: {
        auditTrail: 'Audit Trail',
        emails: 'Emails',
        status: 'Status',
        exceptions: 'Exceptions'
      }
    },
    products: {
      title: 'Products',
      items: {
        characteristics: 'Characteristics',
        products: 'Products',
        allert: 'Allert'
      }
    }
  },
  validationErrorMessages: {
    unableToSearch: 'Unable to search, please try again later.',
    unableToLoadRecords: 'Unable to load records, please try again later.',
    noSearchString: 'Please provide something to search for.',
    searchStringTooLong: 'The value you\'re searching for is too long. Please search for a shorter value.',
    unableToUpdateNotification: 'Unable to update notification, please try again later.',
    unableToListNotifications: 'Unable to get the list of notifications, please try again later.',
    unableToListTenants: 'Unable to get the list of tenants, please try again later.',
    unableToGetAuditTrail: 'Unable to get the audit trail, please try again later.',
    unableToNotify: 'Unable to send this notification, please try again later.',
    unableToCompleteAction: 'Unable to complete this action, please try again later.',
    unableToGetExternalComponentStatuses: 'Unable to get the cached status information of external components, please try again later.',
    unableToGetExceptions: 'Unable to get the exception tracking information, please try again later.',
    noSubject: 'Please provide a subject.',
    subjectTooLong: 'This subject is too long. Please provide a shorter subject.',
    noMessage: 'Please provide a message.',
    unableToUpdateProductCharacteristic: 'Unable to update product characteristic, please try again later.',
    noName: 'Please provide a name.',
    nameTooLong: 'This name is too long. Please provide a shorter name.',
    descriptionTooLong: 'This description is too long. Please provide a shorter description.',
    unableToCreateProductCharacteristic: 'Unable to create product characteristic, please try again later.'
  },
  actions: {
    search: 'Search',
    confirm: 'Confirm',
    cancel: 'Cancel'
  },
  noValue: {
    notificationStartDate: '(not set)',
    notificationEndDate: '(not set)',
    externalTierId: '(not set)',
    tierStartDate: '(not set)',
    tierEndDate: '(not set)'
  },
  dialogs: {
    confirmationDialogHeader: 'Confirmation Required',
    enableUserConfirmation: 'Are you sure you want to enable {thisPlural} user{plural}?',
    disableUserConfirmation: 'Are you sure you want to disable {thisPlural} user{plural}?',
    deleteProductCharacteristicConfirmation: 'Are you sure you want to delete {thisPlural} product characteristic{plural}?'
  },
  tenantList: {
    headers: {
      name: 'Name',
      enabled: 'Membership Enabled?'
    }
  },
  auditTrailEntries: {
    editCharacteristic: {
      fields: {
        id: 'Internal ID',
        creatingCharacteristic: 'Creating Characteristic?',
        name: 'Name',
        description: 'Description',
        enabled: 'Enabled?',
        visible: 'Visible?'
      }
    },
    editProduct: {
      fields: {
        id: 'Internal ID',
        identifier: 'External Identifier',
        creatingProduct: 'Creating Product?',
        name: 'Name',
        description: 'Description',
        moreInformationUrl: 'More Info. URL'
      }
    },
    editTier: {
      fields: {
        id: 'Internal ID',
        externalId: 'Stripe Plan ID',
        creatingTier: 'Creating Tier?',
        name: 'Name',
        productId: 'Internal Product ID',
        visible: 'Visible?',
        startDate: 'Start Date',
        endDate: 'End Date'
      },
      tierCharacteristicValues: {
        fields: {
          name: 'Characteristic',
          value: 'Value'
        }
      }
    },
    editNotification: {
      fields: {
        id: 'Internal ID',
        creatingNotification: 'New Notification?',
        target: 'Target',
        active: 'Active?',
        allowDismissal: 'Allow Dismissal?',
        type: 'Type',
        priority: 'Priority',
        startDate: 'Start Date',
        endDate: 'End Date',
        sendAsEmail: 'Send as Email?',
        emailBatchId: 'Email Batch Message ID',
        subject: 'Subject',
        message: 'Message'
      },
      formatStrings: {
        singleUserTarget: 'User ({name})',
        singleTenantTarget: 'Tenant ({name})'
      }
    }
  },
  notificationList: {
    headers: {
      subject: 'Subject',
      startDate: 'Start Date',
      endDate: 'End Date',
      type: 'Type'
    },
    groupHeaders: {
      allUsers: 'All Users',
      specificUser: 'User: ',
      specificTenant: 'Tenant: ',
      masterTemplate: '{group} ({count} notification{plural})'
    },
    notificationTypes: [
      { id: 1, label: 'Alert' },
      { id: 2, label: 'Information' },
      { id: 3, label: 'Error' },
      { id: 4, label: 'Warning' }
    ],
    notificationStatuses: [
      { id: 1, label: 'Active' },
      { id: 2, label: 'Cancelled' }
    ],
    notificationPriorities: [
      { name: 'low', label: 'Low' },
      { name: 'normal', label: 'Normal' },
      { name: 'high', label: 'High' }
    ]
  },
  forms: {
    updateNotification: {
      title: 'Edit Notification',
      description: 'You can edit the details of this notification using the fields below. When you\'re finished, hit Save.',
      tooltips: {
        allUsers: 'This notification appears to all users of all Jibberwock services.',
        specificUser: 'This notification only appears to this user account.',
        specificTenant: 'This notification only appears to user accounts linked to this tenant.'
      }
    },
    notify: {
      title: 'Notify',
      description: 'You can use the fields below to notify one or more users. When you\'re finished, hit Notify.',
      tooltips: {
        allUsers: 'This notification will appear to all users of all Jibberwock services.',
        specificUser: 'This notification will only appear to this user account.',
        specificTenant: 'This notification will only appear to user accounts linked to this tenant.'
      }
    },
    createProductCharacteristic: {
      title: 'Create Product Characteristic',
      description: 'Fill in the fields below to create a new product characteristic. When you\'re done, click Create.'
    },
    fields: {
      addressedTo: 'Addressed To',
      priority: 'Priority',
      notificationType: 'Type',
      subject: 'Subject',
      message: 'Message',
      active: 'Active',
      allowDismissal: 'Allow Dismissal',
      startDate: 'Start Date',
      endDate: 'End Date',
      sendAsEmail: 'Send as Email',
      name: 'Name',
      valueType: 'Value Type',
      description: 'Description',
      enabled: 'Enabled?',
      visible: 'Visible?'
    },
    buttons: {
      update: 'Save',
      notify: 'Notify',
      cancel: 'Cancel',
      create: 'Create'
    }
  },
  pages: {
    homepage: {
      title: 'Homepage',
      unauthenticated: {
        header: 'Welcome to the Jibberwock Admin Site',
        comment: 'Please log in to this site using the "Log In" link on the left.'
      }
    },
    users: {
      title: 'Users',
      instructions: 'To find users by their name, search below. You can then enable or disable their account and send notifications to them. You can use an asterisk (*) as a wildcard if required.',
      searchTitle: 'Search',
      searchButton: 'Search',
      actions: {
        enable: 'Enable',
        disable: 'Disable',
        notify: 'Notify',
        notifyAll: 'Notify All'
      },
      headers: {
        userName: 'Name',
        enabled: 'Enabled?'
      },
      detailsPanel: {
        title: 'Details',
        notifications: {
          title: 'Notifications'
        },
        tenants: {
          title: 'Tenants'
        }
      },
      errorMessages: {
        selectUserForDetails: 'Search for and select a user (or users) to populate this panel.'
      }
    },
    audit_trail: {
      title: 'Audit Trail',
      instructions: 'Use the filter controls below to view the audit trail.',
      fields: {
        startDate: 'Start Date',
        endDate: 'End Date',
        performedBy: 'Performed By',
        eventType: 'Event Type',
        relatedUser: 'Related User',
        relatedTenant: 'Related Tenant'
      },
      actions: {
        showColumns: 'Columns'
      },
      headers: {
        occurrence: 'Time',
        relatedUser: 'Related User',
        relatedTenant: 'Related Tenant',
        performedBy: 'Performed By',
        originatingConnectionId: 'Request ID',
        originatingService: 'Service',
        type: 'Event Type',
        comment: 'Comment'
      },
      accordionHeaders: {
        generalDetails: 'General',
        specificDetails: 'Details ({type})'
      },
      eventTypes: [
        { id: 1, label: 'Edit User', component: 'EditUser' },
        { id: 2, label: 'Edit Product', component: 'EditProduct' },
        { id: 3, label: 'Delete Product', component: 'EditProduct' },
        { id: 4, label: 'Edit Characteristic', component: 'EditCharacteristic' },
        { id: 5, label: 'Delete Characteristic', component: 'EditCharacteristic' },
        { id: 6, label: 'Edit Tier', component: 'EditTier' },
        { id: 7, label: 'Edit Notification', component: 'EditNotification' }
      ]
    },
    status: {
      title: 'Status',
      tooltip: 'As of {date}',
      externalComponentPurposes: {
        EmailTrackingWebHooks: 'Email Status Tracking',
        DomainRegistrar: 'Domain Registration',
        ContentDistributionNetwork: 'Content Distribution Network',
        Dns: 'DNS',
        ContinuousIntegration: 'Continuous Integration'
      }
    },
    tenants: {
      title: 'Tenants',
      instructions: 'To find tenants by their name, search below. You can then send notifications to their members. You can use an asterisk (*) as a wildcard if required.',
      headers: {
        tenantName: 'Name'
      },
      detailsPanel: {
        title: 'Details',
        notifications: {
          title: 'Notifications'
        }
      },
      errorMessages: {
        selectTenantForDetails: 'Search for and select a tenant (or tenants) to populate this panel.'
      }
    },
    exceptions: {
      title: 'Exceptions',
      jsExceptionsByPage: {
        chartTitle: 'JavaScript errors by page over time',
        detailsTemplate: 'Click or tap on a data point in the "JavaScript errors by page over time" chart to see all errors for a page on a given date.',
        pageTemplate: 'Selected page: ',
        headers: {
          timestamp: 'Timestamp',
          sessionId: 'Session',
          message: 'Error'
        }
      },
      failedRequestsByRoute: {
        chartTitle: 'Bad API requests by route over time',
        detailsTemplate: 'Click or tap on a data point in the "Bad API requests by route over time" chart to see all bad API requests for a route on a given date.',
        pageTemplate: 'Selected route: ',
        headers: {
          timestamp: 'Timestamp',
          resultCode: 'Result Code',
          roleName: 'Azure Resource'
        }
      },
      serverSideErrorsByResource: {
        chartTitle: 'Server-side errors by Azure Resource over time',
        detailsTemplate: 'Click or tap on a data point in the "Server-side errors by Azure Resource over time" chart to see all errors in an Azure resource based upon their operations and messages.',
        pageTemplate: 'Selected resource: ',
        detailsChart: {
          chartTitle: 'Server-side errors for Azure Resource by operation'
        }
      },
      serverSideErrorsByRoute: {
        chartTitle: 'Server-side errors by route over time',
        detailsTemplate: 'Click or tap on a data point in the "Server-side errors by route over time" chart to see all errors for a route on a given date.',
        pageTemplate: 'Selected resource: ',
        headers: {
          timestamp: 'Timestamp',
          type: 'Error Type',
          message: 'Error'
        },
        errorTypeMappings: {
          'System.ComponentModel.DataAnnotations.ValidationException': 'Validation',
          'System.InvalidOperationException': 'Invalid Operation',
          'Microsoft.Azure.AppService.Proxy.Common.Expressions.ExpressionParsingException': 'Expression Parsing',
          'Newtonsoft.Json.JsonReaderException': 'JSON Parsing (Configuration)',
          'System.Net.Sockets.SocketException': 'Socket',
          'System.NullReferenceException': 'Null Reference',
          'System.Data.SqlClient.SqlException': 'MSSQL Client',
          'System.ObjectDisposedException': 'Object Disposed'
        }
      }
    },
    characteristics: {
      title: 'Product Characteristics',
      instructions: 'Review the list of product characteristics below. You can then edit or delete these characteristics, or create a new one.',
      headers: {
        name: 'Name',
        description: 'Description',
        enabled: 'Enabled?',
        visible: 'Visible?',
        valueType: 'Value Type'
      },
      actions: {
        refresh: 'Refresh',
        create: 'Create',
        delete: 'Delete'
      },
      detailsPanel: {
        title: 'Edit'
      },
      errorMessages: {
        selectCharacteristicToEdit: 'Select a product characteristic to populate this panel.'
      },
      characteristicValueTypes: [
        { id: 1, label: 'String' },
        { id: 2, label: 'Boolean' },
        { id: 3, label: 'Numeric' }
      ]
    }
  }
}
