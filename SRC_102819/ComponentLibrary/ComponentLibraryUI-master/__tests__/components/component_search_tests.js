import React from 'react';
import {shallow} from 'enzyme';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinon from 'sinon';
import locale from 'rc-pagination/lib/locale/en_US';
import {ComponentSearch, __RewireAPI__ as ComponentSearchRewired} from '../../src/components/component-search'

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe("Component Search", () => {
  let props, wrapper, onMaterialSearchResultsFetch, onServiceSearchResultsFetch;
  it("should show search keyword", () => {
    onMaterialSearchResultsFetch = sinon.spy();
    onServiceSearchResultsFetch = sinon.spy();
    props = {
      onMaterialSearchResultsFetch: onMaterialSearchResultsFetch,
      onServiceSearchResultsFetch: onServiceSearchResultsFetch,
      componentType: "material",
      keyword: "cement ant",
      result: {isFetching: false},
    };
    wrapper = shallow(<ComponentSearch {...props} />);
    expect(wrapper).to.have.descendants('div');
    expect(wrapper).to.have.descendants('h2');
    expect(wrapper.find('div').first().children().find('h2').first().children().find('span').first())
      .to.have.text("cement ant");
  });

  context("When the result is not loaded", () => {
    beforeEach(() => {
      onMaterialSearchResultsFetch = sinon.spy();
      onServiceSearchResultsFetch = sinon.spy();
      props = {
        onMaterialSearchResultsFetch: onMaterialSearchResultsFetch,
        onServiceSearchResultsFetch: onServiceSearchResultsFetch,
        componentType: "material",
        keyword: "cement ant",
        group: "cement group",
        pageNumber: 1,
      };
      wrapper = shallow(<ComponentSearch {...props} />);
    });
    it("componentDidMount should call onMaterialSearchFetch with groupName", () => {
      wrapper.instance().componentDidMount();
      expect(onMaterialSearchResultsFetch).to.be.calledWith("cement group", "cement ant", 1);
    });
    it("componentWillReceiveProps should call onMaterialSearchFetch with groupName", () => {
      props.pageNumber = 2;
      wrapper.setProps(props);
      expect(onMaterialSearchResultsFetch).to.be.calledWith("cement group", "cement ant", 2);
    });
    context('isFetching', () => {
      beforeEach(() => {
        wrapper.setProps({search: {isFetching: true}});
      });
      it("should show loading screen", () => {
        expect(wrapper).to.have.descendants('div');
        expect(wrapper.find('div').first()).to.have.descendants('Loading');
      });

      it('componentWillReceiveProps should not call onMaterialSearchFetch if search isFetching is true', () => {
        expect(onMaterialSearchResultsFetch).to.not.be.called;
      });
    });

    context('on error', () => {
      it('should call onMaterialSearchFetch', () => {
        wrapper.setProps({error: {type: 'Some'}});
        expect(onMaterialSearchResultsFetch).to.not.have.been.called;
      });
    });
  });

  context("When result is loaded", () => {
    beforeEach(() => {
      props = {
        onMaterialSearchResultsFetch: sinon.spy(),
        onServiceSearchResultsFetch: sinon.spy(),
        group: 'Clay Material',
        keyword: 'clay',
        isListing: false,
        componentType: "material",
        search: {
          isFetching: false,
          values: {
            items: [{
              headers: [
                {
                  columns:[
                    {
                      name: 'Material Code',
                      key: 'material_code',
                      value: 'CLY0001'
                    }
                  ],
                  name: 'General',
                  key: 'general'
                },
                {
                  columns:[
                    {
                      name: 'Material Level 1',
                      key: 'material_level_1',
                      value: 'primary'
                    },
                    {
                      name: 'Material Level 2',
                      key: 'material_level_2',
                      value: 'Clay'
                    }

                  ],
                  name: 'Classification',
                  key: 'classification'
                }
              ]
            }],
            sortColumn: 'Material Name',
            sortOrder: 'Ascending',
            recordCount: 10,
            totalPages: 2,
            batchSize: 5,
          },
        },
        groupName: "cement group",
        onSort: sinon.spy(),
        selectedTab: 'classification',
        onAmendQueryParams: sinon.spy(),
        pageNumber: 1,
      };
      wrapper = shallow(<ComponentSearch {...props} />);
    });

    it('should show filter button', () => {
      const preventDefaultSpy = sinon.spy();
      wrapper = shallow(<ComponentSearch {...props}/>);
      expect(wrapper).to.have.descendants('#filter');
      expect(wrapper.find('#filter')).to.have.text('Filter');
      // wrapper.find('#filter').simulate('click',{preventDefault: preventDefaultSpy});
      // expect(wrapper).to.have.descendants('FilterComponentConnector');
      // expect(wrapper.find('FilterComponentConnector')).to.have.prop('onClose');
      // expect(wrapper.find('FilterComponentConnector')).to.have.prop('group');
      // expect(wrapper.find('FilterComponentConnector')).to.have.prop('componentType');
    });

    it('should display message', () => {
      props = {
        group: 'Clay'
      };
      expect(wrapper).to.include.text('Displaying 1 of 10 records found in');
    });

    it("should render page tabs", () => {
      expect(wrapper).to.have.descendants('PageTab');
      expect(wrapper).to.have.descendants('Link');
      expect(wrapper.find('PageTab')).to.have.prop('selectedTab').equal(props.selectedTab);
    });

    it('should render SortComponent', () => {
      expect(wrapper).to.have.descendants('SortComponent');
      expect(wrapper.find('SortComponent')).to.have.prop('sortOrder').equal('Ascending');
      expect(wrapper.find('SortComponent')).to.have.prop('sortColumn').equal('Material Name');
      const expected = [
        {value: "Material Level 1", key: "material_level_1"}, {value: "Material Level 2", key: "material_level_2"}
      ];
      expect(wrapper.find('SortComponent')).to.have.prop('sortableProperties').deep.equal(expected);
    });

    it('should not render SortComponent when isListing is true', () => {
      props.isListing = true;
      wrapper = shallow(<ComponentSearch {...props} />);
      expect(wrapper).to.not.have.descendants('SortComponent');
    });

    it('should render PageTab with General if selectedTab is not present', () => {
      props.selectedTab = undefined;
      wrapper = shallow(<ComponentSearch {...props} />);
      expect(wrapper.find('PageTab')).to.have.prop('selectedTab', 'general');
    });

    it('should call onAmendQueryParams when onSort is called and componentType is material', () => {
      wrapper.find('SortComponent').simulate('sort', 'Material Level 2', 'Descending');
      expect(props.onAmendQueryParams).to.have.been.calledWith({
        pageNumber: 1,
        sortColumn: 'Material Level 2',
        sortOrder: 'Descending'
      });
    });

    it('should call onAmendQueryParams when onSort is called and componentType is service', () => {
      props.componentType = 'service';
      wrapper = shallow(<ComponentSearch {...props} />);
      wrapper.find('SortComponent').simulate('sort', 'Material Level 2', 'Descending');
      expect(props.onAmendQueryParams).to.have.been.calledWith({
        pageNumber: 1,
        sortColumn: 'Material Level 2',
        sortOrder: 'Descending'
      });
    });

    it('should push new URL with selectedTab when onTabChange is called', () => {
      wrapper.find('PageTab').simulate('tabChange', 'general');
      expect(props.onAmendQueryParams).to.have.been.calledWith({selectedTab: 'general'});
    });

    it('should render Pagination', () => {
      const pagination = wrapper.find('Pagination');
      expect(pagination).to.have.length(1);
      expect(pagination).to.have.prop('current', 1);
      expect(pagination).to.have.prop('total', 10);
      expect(pagination).to.have.prop('pageSize', 5);
      expect(pagination).to.have.prop('locale', locale);
    });

    it('should not render Pagination when isListing is true', () => {
      props.isListing = true;
      wrapper = shallow(<ComponentSearch {...props} />);
      expect(wrapper).to.not.have.descendants('Pagination');
    });

    it('should call onAmendQueryParams with new Page number when pagination calls onChange', () => {
      wrapper.find('Pagination').simulate('change', 2);
      expect(props.onAmendQueryParams).to.have.been.calledWith({pageNumber: 2});
    });

    describe('componentWillReceiveProps', () => {
      it('should call componentWillReceiveProps when pageNumber changes', () => {
        wrapper.setProps({pageNumber: 2});
        expect(props.onMaterialSearchResultsFetch).to.have.been.calledWith(
          props.group,
          props.keyword,
          2,
          props.isListing,
          props.sortColumn,
          props.sortOrder);
      });

      it("should not call onMaterialSearchResultsFetch when filtering page number is " +
        "one and sort column,sort order is not changed", () => {
        props.sortColumn = 'Material Name';
        props.sortOrder = 'Ascending';
        wrapper = shallow(<ComponentSearch {...props} />);
        wrapper.setProps({pageNumber: 1, isFiltering: true, sortColumn: 'Material Name', sortOrder: 'Ascending'});
        expect(props.onMaterialSearchResultsFetch).to.not.have.been.called;
      });

      it("should not call onMaterialSearchResultsFetch when filter columns are empty", () => {
        wrapper = shallow(<ComponentSearch {...props} />);
        expect(props.onMaterialSearchResultsFetch).to.not.have.been.called;
      });

      it("should call onMaterialSearchResultsFetch when filtering page number is " +
        "one and sort column,sort order got changed", () => {
        props.sortColumn = 'Material Name';
        props.sortOrder = 'Descending';
        wrapper = shallow(<ComponentSearch {...props} />);
        wrapper.setProps({pageNumber: 1, isFiltering: true, sortColumn: 'Material Name', sortOrder: 'Ascending'});
        expect(props.onMaterialSearchResultsFetch).to.have.been.called;
      });
    });
  });

  describe("package search", () => {
    let onMaterialSearchResultsFetch, onServiceSearchResultsFetch, onPackageSearchResultsFetch, props, wrapper,
      onPackageSearchDestroy;
    beforeEach(() => {
      onMaterialSearchResultsFetch = sinon.spy();
      onServiceSearchResultsFetch = sinon.spy();
      onPackageSearchResultsFetch = sinon.spy();
      onPackageSearchDestroy = sinon.spy();
      props = {
        onMaterialSearchResultsFetch: onMaterialSearchResultsFetch,
        onServiceSearchResultsFetch: onServiceSearchResultsFetch,
        onPackageSearchResultsFetch: onPackageSearchResultsFetch,
        onPackageSearchDestroy: onPackageSearchDestroy,
        componentType: "package",
        keyword: "pack ant",
        group: "pack group",
        pageNumber: 1,
      };
      wrapper = shallow(<ComponentSearch {...props} />);
    });
    describe("onPackageSearchResultsFetch", () => {
      it("componentDidMount should call onPackageSearchResultsFetch with groupName", () => {
        wrapper.instance().componentDidMount();
        expect(onPackageSearchResultsFetch).to.be.calledWith("pack ant", 1);
      });

      it("componentWillUnmount should call onPackageSearchDestroy", () => {
        wrapper.instance().componentWillUnmount();
        expect(onPackageSearchDestroy).to.be.called;
      });
    });

  });
});


