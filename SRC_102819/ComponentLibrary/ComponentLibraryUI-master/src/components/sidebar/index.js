import React from 'react';
import {sidebar, links, icon, collapsed, link, activeLink, logo, shortLogo, logoContainer, ratesNav, ratesNavLink} from './index.css';
import {Icon} from '../icon';
import classNames from 'classnames';
import {Link} from 'react-router';
import {idFor, isFeatureEnabled} from '../../helpers';
import onClickOutside from 'react-onclickoutside';
import {PermissionedForNonComponents} from "../../permissions/permissions";
import {getViewCPRValuesPermissions} from "../../permissions/ComponentPermissions";

export class SidebarActual extends React.Component {
  constructor(props) {
    super();
    this.state = {collapsed: !!props.collapsed};
  }

  handleClickOutside = () => {
    this.setState({collapsed: true});
  }

  render() {
    return (<div id={idFor('sidebar')}
                 className={classNames(sidebar, this.state.collapsed && collapsed, this.props.className)}
                 onMouseEnter={() => this.setState({collapsed: false})}
                 onMouseLeave={() => this.setState({collapsed: true})}
                 onTouchStart={() => this.setState({collapsed: false})}
                 onClick={() => this.setState({collapsed: false})}
    >
      <Link className={logoContainer} to="/materials">
        <Icon name="squirrel" className={shortLogo}/>
        <Icon name="logo" className={logo}/>
      </Link>
      <ul className={links}>
        <li id={idFor('sidebar', 'materials')}><Link id={idFor('materials')} to="/materials" className={link}
                                                     activeClassName={activeLink}><Icon name="materials"
                                                                                        className={icon}/>{this.state.collapsed ? '' : ' Materials'}
        </Link></li>
        <li id={idFor('sidebar', 'services')}><Link id={idFor('services')} to="/services" className={link}
                                                    activeClassName={activeLink}><Icon name="services"
                                                                                       className={icon}/>{this.state.collapsed ? '' : ' Services'}
        </Link></li>
        {isFeatureEnabled('sfg') && <li id={idFor('sidebar', 'sfg')}><Link id={idFor('sfg')} to="/sfgs" className={link}
                                               activeClassName={activeLink}><Icon name="sfg"
                                                                                  className={icon}/>{this.state.collapsed ? '' : ' SFG'}
        </Link></li>}
        {isFeatureEnabled('packages') && <li id={idFor('sidebar', 'packages')}><Link id={idFor('packages')} to="/packages" className={link}
                                                                           activeClassName={activeLink}><Icon name="packages"
                                                                                                              className={icon}/>{this.state.collapsed ? '' : ' Packages'}
        </Link></li>}

        {isFeatureEnabled('bulkRates') && <li id={idFor('sidebar', 'rates')}>
          <Link id={idFor('rates')} to="/rates" className={link}
                activeClassName={activeLink}>
            <Icon name="rates"
                  className={icon}/>
            {this.state.collapsed ? '' :
              <ul className={ratesNav}>
                <li><Link id={idFor('material', 'rates')} to="/rates/materials" className={ratesNavLink}>Material Rates</Link></li>
                <li><Link id={idFor('service', 'rates')} to="/rates/services" className={ratesNavLink}>Service Rates</Link></li>
              </ul>}
          </Link>
        </li>}
        <li id={idFor('sidebar', 'exchangerates')}><Link id={idFor('exchangerates')} to="/exchange-rates"
                                                         className={link}
                                                         activeClassName={activeLink}><Icon name="exchange"
                                                                                            className={icon}/>{this.state.collapsed ? '' : ' Exchange Rates'}
        </Link></li>
        <PermissionedForNonComponents allowedPermissions={getViewCPRValuesPermissions()}>
        <li id={idFor('sidebar', 'pricebook')}><Link id={idFor('pricebook')} to="/pricebook"
                                                         className={link}
                                                         activeClassName={activeLink}><Icon name="priceBook"
                                                                                            className={icon}/>{this.state.collapsed ? '' : ' Price Book'}
        </Link></li>
        </PermissionedForNonComponents>
      </ul>
    </div>);
  }
}

export const Sidebar = onClickOutside(SidebarActual);
Sidebar.displayName = 'Sidebar';
