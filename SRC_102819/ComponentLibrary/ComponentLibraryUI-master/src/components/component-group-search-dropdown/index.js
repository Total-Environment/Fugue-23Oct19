import React from 'react';
import {dropDown, groups, status, line, link, loading} from './index.css';
import {Link} from 'react-router';
import {Loading} from '../loading'

export class ComponentGroupDropdown extends React.Component {
  searchUrl() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        return '/materials/search';
        break;
      case 'service':
        return '/services/search';
        break;
    }
  }

  renderResults() {
    return <ul className={groups}>
      {this.props.groups.map(group => <li><Link className={link} key={group}
                                                to={`${this.searchUrl()}/${group}/${this.props.keyword}`}>{group}</Link>
      </li>)}
    </ul>;
  }

  render() {
    if(this.props.is_data_loading === 'loading') {
      return <div className={dropDown}>
        <Loading className={loading}/>
      </div>
    }
    else {
      return <div className={dropDown}>
        <h4 className={status}>{this.props.searchResponse}</h4>
        {this.props.groups && this.renderResults()}
      </div>
    }
  }
}
