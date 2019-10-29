import * as React from "react";
import styles from './index.css';
import AnimateHeight from "react-animate-height";
import classNames from 'classnames';
import {PriceBookFilters} from "./filters";
import {idFor, isFetched, toIST} from "../../helpers";
import {Loading} from "../loading/index";
import * as R from "ramda";
import moment from "moment";
import {floatingAction} from '../../css-common/forms.css';
import {PriceBookCreateConnector} from "../pricebook-create/connector";
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";

export const Collapse = ({open, children}) => <AnimateHeight height={ open ? 'auto' : 0 } easing="ease-out"
                                                             duration={100}>
  {children}
</AnimateHeight>;

class CPR extends React.Component {
  constructor(props) {
    super(props);
    this.state = {open: props.defaultOpened && !!this.props.children,isCreateNewCpr: null};
    this.handleClick = this.handleClick.bind(this);
  }

  handleClick() {
    if (!this.props.children) return;
    this.setState({open: !this.state.open});
  }

  render() {
    const indent = 10 + this.props.indent * 15;
    const canOpen = this.props.children.length > 0;
    return <div className={classNames(styles.cpr, {
      [styles.cprClosed]: !this.state.open,
      [styles.projectCpr]: this.props.cpr.projectCode
    })}>
      <div className={classNames(styles.row, {[styles.cprRow]: canOpen})} onClick={this.handleClick}>
        <div className={classNames(styles.column, styles.componentName)} style={{paddingLeft: indent}}>
          {canOpen &&
          <button className={classNames(styles.collapseButton, {[styles.openCollapseButton]: this.state.open})}>
            {this.props.children ? (this.state.open ? '-' : '+') : ' ' }
          </button>} {this.props.name}
        </div>
        {this.props.cpr.cprCoefficient && this.props.cpr.cprCoefficient.columns.map(c => <div
          className={classNames(styles.headerColumn, styles.number)}>{(c.value && c.value.value) || '-'}</div>)}
        <div className={classNames(styles.headerColumn, styles.number)}>
          <strong>{this.props.cpr.cprCoefficient != null ? this.props.cpr.cprCoefficient.cpr : '-'}</strong></div>
        <div
          className={styles.column}>{this.props.cpr.cprCoefficient != null ? toIST(this.props.cpr.appliedFrom, 'DD/MM/YYYY') : null}</div>
      </div>
      <Collapse open={this.state.open}>
        {React.Children.map(this.props.children, (c => React.cloneElement(c, {indent: this.props.indent + 1})))}
      </Collapse>
    </div>;
  }
}
CPR.defaultProps = {indent: 0, defaultOpened: true};

const renderCpr = (name, cpr, opened) => <CPR key={name} name={name} cpr={cpr.self} defaultOpened={opened}>
  {Object.keys(cpr.children).map(child => renderCpr(child, cpr.children[child]))}
</CPR>;

const CPRTable = ({cprs, collapsed}) => <div>
  {R.isEmpty(cprs) ?
    <p className={styles.noCprs}>CPR is not available for this group of components as of the specified date. Please set
      up CPR by clicking the Add button.</p> :
    <div className={styles.cprTable}>
      <div className={styles.header}>
        <div className={styles.headerColumn}>Component</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Quality | Execution Risk (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Custom Production (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Profit (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Corporate | Construction OH (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Design Fee (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Transportation (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Rate Increase (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Profit Margin (%)</div>
        <div className={classNames(styles.headerColumn, styles.number)}>Derived CPR</div>
        <div className={styles.headerColumn}>Applicable From</div>
      </div>
      {Object.keys(cprs).map(cprKey => renderCpr(cprKey, cprs[cprKey], !collapsed))}
    </div>}
</div>;

const replaceCpr = (cpr, newCpr) => {
  if (cpr.projectCode) {
    return;
  } // CPRs with project code will be shown
  cpr.self = newCpr;
};

const nestCprs = (cprs) => {
  const nestedCprs = {};
  cprs.forEach(cpr => {
    const level1 = cpr.level1;
    const level2 = cpr.level2;
    const level3 = cpr.level3;
    const code = cpr.code;

    // Create entries if they are not there
    if (!nestedCprs[level1]) {
      nestedCprs[level1] = {self: {}, children: {}};
    }
    if (level2 && !nestedCprs[level1].children[level2]) {
      nestedCprs[level1].children[level2] = {self: {}, children: {}};
    }
    if (level3 && !nestedCprs[level1].children[level2].children[level3]) {
      nestedCprs[level1].children[level2].children[level3] = {self: {}, children: {}};
    }
    if (code && level3 && !nestedCprs[level1].children[level2].children[level3].children[code]) {
      nestedCprs[level1].children[level2].children[level3].children[code] = {self: {}, children: {}};
    }
    if (code && level3 === null && level2 && !nestedCprs[level1].children[level2].children[code]) {
      nestedCprs[level1].children[level2].children[code] = {self: {}, children: {}};
    }

    // Insert values since there are entries for all
    if (code && level3) {
      replaceCpr(nestedCprs[level1].children[level2].children[level3].children[code], cpr);
    } else if (code && level3 === null && level2) {
      replaceCpr(nestedCprs[level1].children[level2].children[code], cpr);
    } else if (level3) {
      replaceCpr(nestedCprs[level1].children[level2].children[level3], cpr);
    } else if (level2) {
      replaceCpr(nestedCprs[level1].children[level2], cpr);
    } else if (level1) {
      replaceCpr(nestedCprs[level1], cpr);
    }
  });
  return nestedCprs;
};

const componentTypes = ['material', 'service', 'SFG', 'package'];
export class PriceBook extends React.Component {
  constructor(props) {
    super(props);
    this.state = {filterValues: {appliedOn: moment()}};
    this.handlePriceBookFiltersChange = this.handlePriceBookFiltersChange.bind(this);
    this.handleCreateCpr = this.handleCreateCpr.bind(this);
  }

  componentDidMount() {
    if(!this.props.projectsError) {
      this.props.fetchProjectsIfNeeded();
    }
    componentTypes.forEach(c => {
      if(this.props.dependencyDefinitionError[c.toLowerCase()] === '') {return this.props.fetchDependencyDefinitionIfNeeded(c)}
    });
  }

  componentWillUnmount() {
    this.props.onPriceBookDestroy();
  }

  componentWillReceiveProps(props) {
    if(!props.projectsError) {
      this.props.fetchProjectsIfNeeded();
    }
    componentTypes.forEach(c => {
      if(props.dependencyDefinitionError[c.toLowerCase()] === '') {return this.props.fetchDependencyDefinitionIfNeeded(c)}
    });
  }

  handlePriceBookFiltersChange(filterValues) {
    const updatedFilterValues = {...this.state.filterValues, ...filterValues};
    this.setState({filterValues: updatedFilterValues});
    this.fetchCprs(updatedFilterValues);
  }

  fetchCprs(filterValues) {
    if (!this.isFilteredEnoughToFetch(filterValues)) return;
    const filterParams = {
      componentType: filterValues.componentType,
      level1: filterValues.level1 === '' ? undefined : filterValues.level1,
      appliedOn: filterValues.appliedOn && filterValues.appliedOn.toISOString(),
      projectCode: filterValues.projectCode || '',
    };
    this.props.fetchCprs(filterParams);
  }

  isFilteredEnoughToFetch({componentType, level1, appliedOn}) {
    if (!componentType) {
      return false;
    }
    if (componentType.toLowerCase() === 'material' && !level1) {
      return false;
    }
    if (!appliedOn) return false;
    return true;
  }

  isFilteredEnoughToDisplay() {
    if (!this.state.filterValues.componentType) return false;
    if (this.state.filterValues.componentType.toLowerCase() === 'material' && !this.state.filterValues.level1) return false;
    return this.state.filterValues.componentType;
  }

  unprefixMaterialsIfRequired(nestedCprs) {
    if (this.state.filterValues.componentType.toLowerCase() !== 'material') return nestedCprs;
    const children = nestedCprs.Primary || nestedCprs.Secondary;
    return children ? children.children : {};
  }
  fetchProjectName(code) {
    const project = this.props.projects.values.find(p => p.projectCode === code);
    return project && project.shortName;
  }

  showNoProjectExceptionsMessageIfRequired() {
    if(!this.state.filterValues.projectCode) return;
    if(R.any(cpr => cpr.projectCode, this.props.cprs.values)) return;
    return <p className={styles.noCprs}>No Project exceptions are found for {this.fetchProjectName(this.state.filterValues.projectCode)}.</p>;
  }

  shouldCollapseTopLevel() {
    let {level1, level2, projectCode} = this.state.filterValues;
    if (projectCode) return false;
    if (this.state.filterValues.componentType.toLowerCase() === 'material') {
      level1 = this.state.filterValues.level2;
      level2 = this.state.filterValues.level3;
    }
    return !level1 && !level2;
  }

  handleButtonClk() {
    this.setState({isCreateNewCpr: true});
  }

  onCloseCrateCprModal(thisComponent) {
    thisComponent.setState({isCreateNewCpr: false});
  }

  handleCreateCpr() {
    this.fetchCprs(this.state.filterValues);
  }

  render() {
    componentTypes.forEach(c => {
      if(this.props.dependencyDefinitionError[c.toLowerCase()] !== '') {
        return <h3 id={idFor('error')}>this.props.dependencyDefinitionError[c]</h3>;
      }
    });
    if(this.props.cprError || this.props.projectsError) {
      return <h3 id={idFor('error')}>{this.props.cprError || this.props.projectsError}</h3>;
    }
    else if (!R.all(c => isFetched(this.props.dependencyDefinitions[`${c.toLowerCase()}Classifications`]), componentTypes) || !isFetched(this.props.projects)) {
      return <Loading />;
    }
    return <div className={styles.priceBook}>
      <PriceBookFilters filters={this.state.filterValues} dependencyDefinitions={this.props.dependencyDefinitions}
                        projects={this.props.projects.values}
                        onChange={this.handlePriceBookFiltersChange}/>
      {isFetched(this.props.cprs) && this.isFilteredEnoughToDisplay() && <div>
        {this.showNoProjectExceptionsMessageIfRequired()}
        <CPRTable cprs={this.unprefixMaterialsIfRequired(this.applyFilters(nestCprs(this.props.cprs.values)))} collapsed={this.shouldCollapseTopLevel()}/>
      </div>
      }
      {this.props.cprs && this.props.cprs.isFetching && <Loading/>}
      {this.state.isCreateNewCpr == true ? <PriceBookCreateConnector onCloseCrateCprModal={this.onCloseCrateCprModal}
                                                                     callingComponent={this} onCreateCpr={this.handleCreateCpr}/> : undefined}
      <PermissionedForNonComponents allowedPermissions={[permissions.SETUP_CPR_VERSION, permissions.VIEW_CPR_VALUES]}>
      <button className={floatingAction} onClick={(val)=>this.handleButtonClk()}>+</button>
      </PermissionedForNonComponents>
    </div>
  }


  applyFilters(nestedCprs) {
    const {level1, level2, level3} = this.state.filterValues;
    return this.filterCpr({children: nestedCprs}, [level1, level2, level3]).children;
  }

  filterCpr(cpr, keys) {
    if (keys.length === 0) return cpr;
    const key = R.head(keys);
    if (!key) return cpr;
    const filteredChildren = cpr.children[key] ? {[key]: this.filterCpr(cpr.children[key], R.tail(keys))} : {};
    return {self: cpr.self, children: filteredChildren};
  }
}
