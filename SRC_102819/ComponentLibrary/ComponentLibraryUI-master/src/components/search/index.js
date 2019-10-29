import React from 'react';
import styles from './index.css';
import {Icon} from '../icon';
import {ComponentGroupDropdown} from '../component-group-search-dropdown';
import {apiUrl, logException} from '../../helpers';
import classNames from 'classnames';
import axios from 'axios';
import {idFor} from '../../helpers';
import { browserHistory } from 'react-router';

export class Search extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      keywordToSearch: this.props.keyword,
      searchResponse: undefined,
      groups: undefined
    };
  }

  async componentDidMount() {
    if (this.state.keywordToSearch && this.state.keywordToSearch.length > 0) {
      return await this.onComponentSearch(this.validKeywords());
    }
  }

  async onComponentSearch(keyword) {
    if(keyword.length === 0) {
        let response = 'Search keyword should have minimum 3 letters.';
        const details = {searchResponse: response,groups: undefined};
        this.setState(details);
    }
    else {
      this.setState({fetching: true});
      switch (this.props.componentType.toLowerCase()) {
        case 'material':
          await this.onMaterialGroupSearch(keyword);
          break;
        case 'service':
          await this.onServiceGroupSearch(keyword);
          break;
        case 'sfg':
          await this.onSfgSearch(keyword);
          break;
        case 'package':
          await this.onPackageSearch(keyword);
          break;
      }
      this.setState({fetching: false});
      return;
    }
  }

  async onMaterialGroupSearch(keyword) {
    let response = undefined;
    let groups = undefined;
    try {
      const responseResult = await axios.get(apiUrl(`materials/group/${keyword}`));
       if (responseResult.status === 200) {
        groups = responseResult.data;
        response = `The following material groups have search results for "${keyword}":`;
      }
    } catch (error) {
      if (error.response && error.response.status === 404) {
        response = `\'${keyword}\' not found in any material group.`;
      } else if (error.response && error.response.status === 400) {
        response = 'Search keyword should be minimum 3 letter long.';
      } else if (error.response && error.response.status === 500) {
        response = 'Some thing went wrong.';
      }
      else {
        response = 'Some thing went wrong.';
      }
      logException(error);
    } finally {
      const details = {searchResponse: response, groups: groups};
      this.setState(details);
    }
  };

  async onServiceGroupSearch(keyword) {
    let response = undefined;
    let groups = undefined;
    try {
      const responseResult = await axios.get(apiUrl('services/group-search') + "?searchQuery=" + keyword);
      if (responseResult.status === 200) {
        groups = responseResult.data;
        response = `The following service groups have search results for "${keyword}":`;
      }
    } catch (error) {
      if (error.response && error.response.status === 404) {
        response = `\'${keyword}\' not found in any service group.`;
      } else if (error.response && error.response.status === 400) {
        response = 'Search keyword should be minimum 3 letter long.';
      } else if (error.response && error.response.status === 500) {
        response = 'Some thing went wrong.';
      }
      else {
        response = 'Some thing went wrong.';
      }
      logException(error);
    } finally {
      const details = {searchResponse: response, groups: groups};
      this.setState(details);
    }
  };

  async onSfgSearch(keyword) {
    browserHistory.push(`/sfgs/search/${keyword}`);
  }

  async onPackageSearch (keyword) {
    browserHistory.push(`/packages/search/${keyword}`);
  }

  renderDropDown(loading) {
    return [<br/>,
      <ComponentGroupDropdown componentType={this.props.componentType} searchResponse={this.state.searchResponse}
                              groups={this.state.groups}
                              keyword={this.validKeywords()}
                              is_data_loading={loading}
      />];
  }

  renderDropDownIfNeeded() {
    if(this.state.fetching) {
      return this.renderDropDown('loading');
    }
    if(this.state.searchResponse && this.state.focused) {
      return this.renderDropDown();
    } else {
      return;
    }
  }

  validKeywords() {
    return this.state.keywordToSearch.split(' ')
      .filter(w => w.length >= 3)
      .reduce((s, b) => s + ' ' + b, '').trim();
  }

  handleInputFocus(state) {
    this.handleFormFocus(state, this.state.buttonFocused);
  }

  handleButtonFocus(state) {
    this.handleFormFocus(this.state.inputFocused, state);
  }

  handleFormFocus(inputFocused, buttonFocused) {
    this.setState({inputFocused, buttonFocused, focused: inputFocused || buttonFocused});
  }

  renderNormalInput() {
    return [
      <input type="text" className={classNames({[styles.input]: true, [styles.inputFocused]: this.state.focused})}
             key="input"
             id={idFor('keyword to search')}
             onFocus={() => this.handleInputFocus(true)}
             onBlur={() => setTimeout(() => this.handleInputFocus(false), 200)}
             value={this.state.keywordToSearch}
             onChange={(e) => {
               this.setState({keywordToSearch: e.target.value, searchResponse: undefined})
             }}
      />,
      <button className={classNames({[styles.iconContainer]: true, [styles.iconContainerFocused]: this.state.focused})}
              key="button"
              id={idFor('search button')}
              onFocus={() => this.handleButtonFocus(true)}
              onBlur={() => setTimeout(() => this.handleButtonFocus(false), 200)}
              onClick={async(e) => {
                e.preventDefault();
                return await this.onComponentSearch(this.validKeywords());
              }}
      >
        <Icon name="search" className={classNames({
          [styles.icon]: true,
          [this.props.className]: true,
          [styles.iconFocused]: this.state.focused
        })}/>
      </button>
    ];
  }

  render() {
    return <div className={styles.searchBar}>
      <form autoComplete="off" className={classNames({[styles.form]: true, [styles.focusedForm]: this.state.focused})}
            onSubmit={async(e) => {
              e ? e.preventDefault() : undefined;
              return await this.onComponentSearch(this.validKeywords());
            }}>
        { this.renderNormalInput() }
      </form>
      {this.renderDropDownIfNeeded()}
    </div>;
  }
}
