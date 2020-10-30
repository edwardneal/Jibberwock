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
    noSearchString: 'Please provide something to search for.',
    searchStringTooLong: 'The value you\'re searching for is too long. Please search for a shorter value.',
    unableToUpdateNotification: 'Unable to update notification, please try again later.',
    noSubject: 'Please provide a subject.',
    subjectTooLong: 'This subject is too long. Please provide a shorter subject.',
    noMessage: 'Please provide a message.'
  },
  actions: {
    search: 'Search'
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
      sendAsEmail: 'Send as Email'
    },
    buttons: {
      update: 'Save',
      cancel: 'Cancel'
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
    }
  }
}
