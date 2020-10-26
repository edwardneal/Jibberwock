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
    searchStringTooLong: 'The value you\'re searching for is too long. Please search for a shorter value.'
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
    notificationTypes: {
      1: 'Alert',
      2: 'Information',
      3: 'Error',
      4: 'Warning'
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
